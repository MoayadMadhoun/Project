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
using Project.Repostory;
using static Project.Models.TrainingApplication;

namespace Project.Pages.University
{
    [Authorize(Roles = "UniversityTrainingAdmin, DepartmentHead")]
    public class StudentsModel : PageModel
    {


        private readonly UniversityRepository _universityRepo;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly StudentsRepository _studentRepo;

        public StudentsModel(UniversityRepository universityRepo, UserManager<AspNetUser> userManager, ApplicationDbContext dbContext, StudentsRepository studentRepo)
        {
            _universityRepo = universityRepo;
            _userManager = userManager;
            _dbContext = dbContext;
            _studentRepo = studentRepo;
        }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        public Models.University CurrentUniversity { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SelectedSpecialityId { get; set; }
        public SelectList SpecialitiesList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SelectedDepartmentId { get; set; }
        public SelectList DepartmentsList { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool IsTraining { get; set; }
        public PaginatedList<Models.Student> Students { get; set; } = new PaginatedList<Models.Student>(new List<Models.Student>(), 0, 1, 10);
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                AspNetRoleScope? scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                if (scope.UniversityID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                int universityId = (int)scope.UniversityID;

                CurrentUniversity = await _universityRepo.GetByIdAsync(universityId);
                if (CurrentUniversity == null)
                {
                    return RedirectToPage("/Index");
                }
                var query = _studentRepo.GetStudentsForUniversity(universityId);

                if (User.IsInRole("DepartmentHead"))
                {
                    query = query.Where(s => s.DepartmentID == scope.DepartmentID);
                }
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.Name.Contains(SearchTerm));
                }
                if (SelectedSpecialityId > -1)
                {
                    query = query.Where(s => s.SpecialtyID == SelectedSpecialityId);
                }
                if (SelectedDepartmentId > -1)
                {
                    query = query.Where(s => s.DepartmentID == SelectedDepartmentId);
                }
                if (IsTraining)
                {
                    query = query.Where(s => s.Applications.Any(a =>
                        a.Status == ApplicationStatus.Placed));
                }
                else if (IsTraining == false)
                {
                    query = query.Where(s => !s.Applications.Any(a =>
                        a.Status == ApplicationStatus.Placed));
                }
                
                query = SortOrder switch
                {
                    "Name" => query.OrderBy(a => a.Name),
                    "Name_desc" => query.OrderByDescending(a => a.Name),
                    "GPA" => query.OrderBy(a => a.GPA),
                    "GPA_desc" => query.OrderByDescending(a => a.GPA),
                    _ => query.OrderBy(p => p.StudentNumber),
                };
                Students = await PaginatedList<Models.Student>.CreateAsync(query, PageIndex, PageSize);
                return Page();
            }
            catch
            {
                return RedirectToPage("/Index");
            }

        }
    }
}

