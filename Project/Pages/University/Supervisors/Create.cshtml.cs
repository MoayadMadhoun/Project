using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repositories;
using Project.Services;
using Project.ViewModel;

namespace Project.Pages.University.Supervisors
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class CreateModel : PageModel
    {

        private readonly CreateUserService _createUserService; 
        private readonly UserManager<AspNetUser> _userManager;
        
        private readonly ApplicationDbContext _dbcontext;

        public CreateModel(
            CreateUserService createUserService,
            UserManager<AspNetUser> userManager,
            DepartmentRepository repDeparrtment,
            ApplicationDbContext dbcontext)
        {
            _createUserService = createUserService;
            _userManager = userManager;
            
            _dbcontext = dbcontext;
        }



        [BindProperty]
        public SupervisorsVM Input { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();


            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var scope = await _dbcontext.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == currentUser.Id &&
                    x.IsActive);



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
                        RoleName = "UniversitySupervisor",
                        
                        UniversityId = scope?.UniversityID.Value,
                    });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                
                return Page();
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToPage("/University/Index");
        }


    }
}
