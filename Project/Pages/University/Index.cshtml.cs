using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;
using Project.Repostory;

namespace Project.Pages.University
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _universityRepo;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)]
        public Models.University CurrentUniversity { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; }
        public SelectList StatusList { get; set; }
        private readonly UserManager<AspNetUser> _userManager;

        public PaginatedList<TrainingApplication> TrainingApplications { get; set; }
        public IndexModel(ApplicationDbContext dbContext, UniversityRepository universityRepo, UserManager<AspNetUser> userManager)
        {

            _dbContext = dbContext;
            _universityRepo = universityRepo;
            _userManager = userManager;
        }
        public async Task<ActionResult> OnGet()
        {
            TrainingApplications = new PaginatedList<TrainingApplication>(new List<TrainingApplication>(), 0, PageIndex, PageSize);
            try
            {
                if (User == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }

                AspNetRoleScope scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }

                if (scope.UniversityID == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }

                int universityId = (int)scope.UniversityID;

                CurrentUniversity = await _universityRepo.GetByIdAsync(universityId);
                if (CurrentUniversity == null)
                {
                    return RedirectToPage("/Index");
                }
                var query = _universityRepo.GetApplicationsForUniversity(universityId);
                var unis = await _universityRepo.GetAllAsync();
                var selectList = Enum.GetValues(typeof(TrainingApplication.ApplicationStatus))
                    .Cast<TrainingApplication.ApplicationStatus>()
                    .Select(s => new { Value = s.ToString(), Text = s.ToString() });

                StatusList = new SelectList(selectList, "Value", "Text");

                if (StatusId > 0)
                {
                    query = query.Where(a => a.Status == (TrainingApplication.ApplicationStatus)StatusId);
                }
                if (SearchTerm != null)
                {
                    query = query.Where(a => a.TrainingOpportunity.Title.Contains(SearchTerm));
                }

                query = SortOrder switch
                {
                    "Date" => query.OrderBy(a => a.AppliedAt),
                    "Date_desc" => query.OrderByDescending(a => a.AppliedAt),
                    "Status" => query.OrderBy(a => a.Status),
                    "Status_desc" => query.OrderByDescending(a => a.Status),
                    _ => query.OrderBy(p => p.Student.Name),

                };
                TrainingApplications = await PaginatedList<TrainingApplication>.CreateAsync(query, PageSize, PageIndex);
                return Page();

            }
            catch
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
