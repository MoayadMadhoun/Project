using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using Project.Repostory;

namespace Project.Pages.University
{
    [Authorize(Roles ="UniversityTrainingAdmin, DepartmentHead")]
    public class DepartmentsModel : PageModel
    {
        private readonly DepartmentRepository _deptRepo;
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _universityRepo;
        private readonly UserManager<AspNetUser> _userManager;


        public DepartmentsModel(DepartmentRepository deptRepo, ApplicationDbContext dbContext, UniversityRepository universityRepo, UserManager<AspNetUser> userManager)
        {
            _deptRepo = deptRepo;
            _dbContext = dbContext;
            _universityRepo = universityRepo;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = string.Empty;
        public Models.University CurrentUniversity { get; set; } = new Models.University();
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public bool? IsActive { get; set; }
        [BindProperty(SupportsGet = true)]
        public int RemoveDeptId { get; set; }

        public PaginatedList<Department> Departments { get; set; } = new PaginatedList<Department>(new List<Department>(), 1, 0, 10);

        public async Task<IActionResult> OnGet()
        {
            
            try
            {
                
                var user = await _userManager.GetUserAsync(User);
                AspNetRoleScope scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

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
                var query = _deptRepo.GetAllQueryable().Where(d => d.UniversityID == universityId);
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.Name.Contains(SearchTerm));
                }
                if (IsActive == true)
                {
                    query = query.Where(a => a.IsActive);
                }
                else if (IsActive == false)
                {
                    query = query.Where(a => !a.IsActive);
                }
               
                query = SortOrder switch
                {
                    "StuCount" => query.OrderBy(a => a.Students.Count),
                    "StuCount_desc" => query.OrderByDescending(a => a.Students.Count),
                    _ => query.OrderBy(p => p.DepartmentID),

                };
                Departments = await PaginatedList<Department>.CreateAsync(query, PageSize, PageIndex);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }

        }
     
        public async Task<IActionResult> OnPostDeleteAsync(int RemoveDeptId)
        {
            try
            {
                if (User == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                var user = await _userManager.GetUserAsync(User);
                AspNetRoleScope scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);
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
                var deptToRemove = await _deptRepo.GetByIdAsync(RemoveDeptId);
                if (deptToRemove != null && deptToRemove.UniversityID == universityId)
                {
                    await _deptRepo.FullDeleteDepartment(RemoveDeptId);
                }
                return RedirectToPage(new { SearchTerm, IsActive, SortOrder, PageIndex, PageSize });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
