using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using Project.Repository;

namespace Project.Pages.University.Specialties
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SpecialtyRepository _specialtyRepository;

        public IndexModel(
            UserManager<AspNetUser> userManager,
            ApplicationDbContext context,
            SpecialtyRepository specialtyRepository)
        {
            _userManager = userManager;
            _context = context;
            _specialtyRepository = specialtyRepository;
        }

      

        public int TotalSpecialties { get; set; }

        public int ActiveSpecialties { get; set; }

        public int InactiveSpecialties { get; set; }

        public int TotalStudents { get; set; }


        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int DepartmentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public SelectList Departments { get; set; }

        //public PaginatedList<Specialty> FSpecialties { get; set; }


        public PaginatedList<Specialty> Specialties { get; set; }

        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }

        public async Task<IActionResult> OnGetAsync()
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login",
                    new { area = "Identity" });

            var scope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == user.Id &&
                    x.IsActive);

            if (scope?.UniversityID == null)
                return RedirectToPage("/Index");

            int universityId = scope.UniversityID.Value;

            // Departments Filter

            var departments = await _context.Departments
                .Where(d => d.UniversityID == universityId)
                .OrderBy(d => d.Name)
                .ToListAsync();

            Departments = new SelectList(
                departments,
                "DepartmentID",
                "Name");

            var query = _specialtyRepository
                .GetUniversitySpecialtiesQueryable(universityId);

            // Search

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(s =>
                    s.Name.Contains(SearchTerm) ||
                    s.Description!.Contains(SearchTerm));
            }

            // Department Filter

            if (DepartmentId > 0)
            {
                query = query.Where(s =>
                    s.DepartmentID == DepartmentId);
            }

            // Status Filter

            if (StatusId > 0)
            {
                bool active = StatusId == 1;

                query = query.Where(s =>
                    s.IsActive == active);
            }

            // الإحصائيات
            TotalSpecialties = await query.CountAsync();

            ActiveSpecialties = await query
                .CountAsync(x => x.IsActive);

            InactiveSpecialties = await query
                .CountAsync(x => !x.IsActive);

            TotalStudents = await query
                .SumAsync(x => x.Students.Count);

            // Sorting

            query = SortOrder switch
            {
                "Name_desc" => query.OrderByDescending(x => x.Name),

                "Department" => query.OrderBy(x => x.Department.Name),

                "Department_desc" => query.OrderByDescending(x => x.Department.Name),

                "Students" => query.OrderBy(x => x.Students.Count),

                "Students_desc" => query.OrderByDescending(x => x.Students.Count),

                _ => query.OrderBy(x => x.Name)
            };

            Specialties = await PaginatedList<Specialty>
                .CreateAsync(query, PageSize, PageIndex);

            return Page();
        }
    }
}