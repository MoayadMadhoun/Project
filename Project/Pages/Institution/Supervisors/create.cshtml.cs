using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Models;
using Project.Pages.Institution.ViewModels;
using Project.Services;

namespace Project.Pages.Institution.Supervisors
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class createModel : PageModel
    {
        private readonly CreateUserService _createUserService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _context;

        public createModel(
            CreateUserService createUserService,
            UserManager<AspNetUser> userManager,
            ApplicationDbContext context)
        {
            _createUserService = createUserService;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public AddSupervisorVM Input { get; set; }


        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login",new { area = "Identity" });
            }

            var scope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == currentUser.Id &&
                    x.IsActive);

            if (scope?.InstitutionID == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _createUserService.CreateUserAsync(
                    new CreateUserRequest
                    {
                        FullName = Input.FullName,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        IsActive = false,
                        RoleName = "InstitutionSupervisor",
                        InstitutionId = scope.InstitutionID.Value
                    });

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty,result.Message);
                return Page();
            }

            TempData["SuccessMessage"] =result.Message;
            return RedirectToPage("/Institution/Supervisors");
        }
    }
}
