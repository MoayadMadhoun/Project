using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Models;
using Project.Repositories;
using Project.Services;
using Project.ViewModel;
using System.Threading.Tasks;

namespace Project.Pages.University.DepartmentHeads
{
    [Authorize (Roles = "UniversityTrainingAdmin")]
    public class CreateModel : PageModel
    {
        private readonly CreateUserService _createUserService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly DepartmentRepository _repDeparrtment;
        private readonly ApplicationDbContext _dbcontext;

        public CreateModel(
            CreateUserService createUserService,
            UserManager<AspNetUser> userManager,
            DepartmentRepository repDeparrtment,
            ApplicationDbContext dbcontext)
        {
            _createUserService = createUserService;
            _userManager = userManager;
            _repDeparrtment = repDeparrtment;
            _dbcontext = dbcontext;
        }

       

        [BindProperty]
        public DepartmentHeadsVM Input { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public List<SelectListItem> DepartmentList { get; set; }
        public async Task OnGet()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var scope = await _dbcontext.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == currentUser.Id &&
                    x.IsActive);

            DepartmentList = await _repDeparrtment.GetUniversityDepartmentsSelectListAsync(scope.UniversityID.Value);
        }

        public async Task<IActionResult> OnPost()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var scope = await _dbcontext.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == currentUser.Id &&
                    x.IsActive);

            if (!ModelState.IsValid)
            {
                DepartmentList = await _repDeparrtment.GetUniversityDepartmentsSelectListAsync(scope.UniversityID.Value);
                return Page();
            }
              

           

            if (scope?.UniversityID == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _createUserService.CreateUserAsync(
                    new CreateUserRequest
                    {
                        FullName = Input.Name,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber!,
                        IsActive = true,
                        RoleName = "DepartmentHead",
                        DepartmentId = Input.DepartmentId,
                        UniversityId=scope?.UniversityID.Value,
                    });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                DepartmentList = await _repDeparrtment.GetUniversityDepartmentsSelectListAsync(scope.UniversityID.Value);
                return Page();
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToPage("/University/Index");

        }
    }
}
