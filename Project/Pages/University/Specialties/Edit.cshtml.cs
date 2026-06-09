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
using Project.Repository;

namespace Project.Pages.University.Specialties
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly DepartmentRepository _departmentRepository;

        public EditModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager,
            DepartmentRepository departmentRepository)
        {
            _context = context;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
        }

        [BindProperty]
        public EditSpecialtyVM Input { get; set; }

        public List<SelectListItem> Departments { get; set; }
            = new();

        public int StudentCount { get; set; }

        public int OpportunityCount { get; set; }

        public int RequestCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await LoadPageDataAsync();

            var specialty = await _context.Specialties
                .Include(x => x.Students)
                .Include(x => x.OpportunitySpecialties)
                .Include(x => x.RequestSpecialties)
                .FirstOrDefaultAsync(x => x.SpecialtyID == id);

            if (specialty == null)
                return NotFound();

            Input = new EditSpecialtyVM
            {
                SpecialtyID = specialty.SpecialtyID,
                Name = specialty.Name,
                Description = specialty.Description,
                Category = specialty.Category,
                DepartmentID = specialty.DepartmentID,
                IsActive = specialty.IsActive
            };

            StudentCount = specialty.Students.Count;
            OpportunityCount = specialty.OpportunitySpecialties.Count;
            RequestCount = specialty.RequestSpecialties.Count;

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
                return RedirectToPage("/Index");

            bool departmentBelongsToUniversity =
                await _context.Departments
                .AnyAsync(d =>
                    d.DepartmentID == Input.DepartmentID &&
                    d.UniversityID == scope.UniversityID);

            if (!departmentBelongsToUniversity)
            {
                ModelState.AddModelError(
                    nameof(Input.DepartmentID),
                    "القسم المختار غير تابع للجامعة");

                return Page();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(x =>
                    x.SpecialtyID == Input.SpecialtyID);

            if (specialty == null)
                return NotFound();

            bool duplicatedName = await _context.Specialties
                .AnyAsync(x =>
                    x.SpecialtyID != Input.SpecialtyID &&
                    x.DepartmentID == Input.DepartmentID &&
                    x.Name == Input.Name);

            if (duplicatedName)
            {
                ModelState.AddModelError(
                    nameof(Input.Name),
                    "يوجد تخصص بنفس الاسم داخل هذا القسم");

                return Page();
            }

            specialty.Name = Input.Name.Trim();
            specialty.Description = Input.Description;
            specialty.Category = Input.Category;
            specialty.DepartmentID = Input.DepartmentID;
            specialty.IsActive = Input.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "تم تعديل التخصص بنجاح";

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