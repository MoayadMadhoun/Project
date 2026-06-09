using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repositories;

namespace Project.Pages.University.Specialties
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly DepartmentRepository _departmentRepository;

        public CreateModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager,
            DepartmentRepository departmentRepository)
        {
            _context = context;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
        }

        [BindProperty]
        public CreateSpecialtyVM Input { get; set; }

        public List<SelectListItem> Departments { get; set; }
            = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPageDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadPageDataAsync();

            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });
            }

            var scope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == user.Id &&
                    x.IsActive);

            if (scope?.UniversityID == null)
            {
                return RedirectToPage("/Index");
            }

            bool departmentBelongsToUniversity =
                await _context.Departments
                .AnyAsync(d =>
                    d.DepartmentID == Input.DepartmentID &&
                    d.UniversityID == scope.UniversityID);

            if (!departmentBelongsToUniversity)
            {
                ModelState.AddModelError(
                    nameof(Input.DepartmentID),
                    "القسم المختار غير تابع لجامعتك");

                return Page();
            }

            bool specialtyExists = await _context.Specialties.AnyAsync(s =>
                s.DepartmentID == Input.DepartmentID &&
                s.Name == Input.Name);

            if (specialtyExists)
            {
                ModelState.AddModelError(nameof(Input.Name),"يوجد تخصص بنفس الاسم داخل هذا القسم");
                return Page();
            }

            var specialty = new Specialty
            {
                Name = Input.Name.Trim(),
                Description = Input.Description,
                Category = Input.Category,
                DepartmentID = Input.DepartmentID,
                IsActive = Input.IsActive
            };

            _context.Specialties.Add(specialty);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "تم إنشاء التخصص بنجاح";

            return RedirectToPage("Index");
        }


        private async Task LoadPageDataAsync()
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
                return;

            var scope =
                await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == user.Id &&
                    x.IsActive);

            if (scope?.UniversityID == null)
                return;

            Departments =
                await _departmentRepository
                .GetUniversityDepartmentsSelectListAsync(
                    scope.UniversityID.Value);
        }
    }
}