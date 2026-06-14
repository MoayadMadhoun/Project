using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.University
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public ProfileModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Project.Models.University University { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });

            var scope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == user.Id &&
                    x.IsActive);

            if (scope?.UniversityID == null)
                return Forbid();

            University = await _context.Universities
                .FirstOrDefaultAsync(x =>
                    x.UniversityID ==
                    scope.UniversityID.Value);

            if (University == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var university = await _context.Universities
                .FirstOrDefaultAsync(x =>
                    x.UniversityID ==
                    University.UniversityID);

            if (university == null)
                return NotFound();

            university.Name = University.Name;
            university.Address = University.Address;
            university.PhoneNumber = University.PhoneNumber;
            university.Email = University.Email;
            university.IsActive = University.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "تم تحديث بيانات الجامعة بنجاح";

            return RedirectToPage();
        }
    }
}