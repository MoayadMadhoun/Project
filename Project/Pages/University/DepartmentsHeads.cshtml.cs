using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using Project.Repostory;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.Pages.University
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class DepartmentsHeadsModel : PageModel
    {
       
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _universityRepo;
        private readonly UserManager<AspNetUser> _userManager;
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = string.Empty;
        public Models.University CurrentUniversity { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public bool IsActive { get; set; }
        public PaginatedList<AspNetUser> DepartmentsHeads { get; set; } = new PaginatedList<AspNetUser>(new List<AspNetUser>(), 1, 0, 10);
        public string DepartmentHeadId { get; set; }

        public DepartmentsHeadsModel(ApplicationDbContext dbContext, UniversityRepository universityRepo, UserManager<AspNetUser> userManager)
        {
          
            _dbContext = dbContext;
            _universityRepo = universityRepo;
            _userManager = userManager;
        }

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
                var query = _universityRepo.GetDepartmentHeadsForUniversity(universityId);
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.FullName.Contains(SearchTerm));
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
                    "Name" => query.OrderBy(a => a.FullName),
                    "Name_desc" => query.OrderByDescending(a => a.FullName),
                    _ => query.OrderBy(p => p.Id),

                };
                DepartmentsHeads = await PaginatedList<AspNetUser>.CreateAsync(query, PageSize, PageIndex);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }

        }
        public async Task<IActionResult> OnPostRemoveDeptHeadAsync(int id)
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
                if (!string.IsNullOrEmpty(DepartmentHeadId))
                {
                    AspNetUser headToRemove = await _userManager.FindByIdAsync(DepartmentHeadId);
                    if (headToRemove != null) {
                        await _userManager.RemoveFromRoleAsync(headToRemove, "DepartmentHead");
                    }
                    
                }

                return RedirectToPage(new { SearchTerm, SortOrder, PageSize, PageIndex });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
