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
        public PaginatedList<AspNetRoleScope> DepartmentsHeads { get; set; } 
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
                    query = query.Where(a => a.User.FullName.Contains(SearchTerm));
                }
                
                query = SortOrder switch
                {
                    "Name" => query.OrderBy(a => a.User.FullName),
                    "Name_desc" => query.OrderByDescending(a => a.User.FullName),
                    _ => query.OrderBy(p => p.UserID),

                };
                DepartmentsHeads = await PaginatedList<AspNetRoleScope>.CreateAsync(query, PageIndex, PageSize);
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

                return RedirectToPage(new { SearchTerm, SortOrder, PageIndex, PageSize });
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
