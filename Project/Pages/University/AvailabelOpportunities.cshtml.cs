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
    public class AvailabelOpportunitiesModel : PageModel
    {
        private readonly UniversityRepository _uniRepo;
        private readonly ApplicationDbContext _dbContext;

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
        [BindProperty(SupportsGet = true)]
        public int SpecialtyId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PeriodMonths { get; set; }
        public SelectList Specialities { get; set; }
        private readonly UserManager<AspNetUser> _userManager;

        public PaginatedList<TrainingOpportunityRequest> OpportunityRequests { get; set; }
        public AvailabelOpportunitiesModel(UniversityRepository uniRepo, ApplicationDbContext dbContext, UserManager<AspNetUser> userManager)
        {
            _uniRepo = uniRepo;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<ActionResult> OnGet()
        {
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

                var scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {

                    return RedirectToPage("Identity/Account/Login");
                }
                //int InstituationID = scope.InstitutionID;


                int universityId = scope.UniversityID.GetValueOrDefault();
                if (universityId == 0)
                {
                    return RedirectToPage("Identity/Account/Login");
                }

                CurrentUniversity = await _uniRepo.GetByIdAsync(universityId);
                if (CurrentUniversity == null)
                {
                    return RedirectToPage("/Index");
                }

                var query = _uniRepo.GetOpportunityRequestForUniversity(universityId);
                var specialities = _dbContext.Specialties.ToList();
                Specialities = new SelectList(specialities, "Id", "Name");
                if (StatusId >= 0)
                {
                    query = query.Where(a => a.Status == (TrainingOpportunityRequest.RequestStatus)StatusId);
                }
                if (SearchTerm != null)
                {
                    query = query.Where(a => a.Title.Contains(SearchTerm));
                }
                if (PeriodMonths > 0)
                {
                    query = query.Where(a => ((a.PreferredEndDate.HasValue? a.PreferredEndDate.Value.Month: 0  ) 
                    - (a.PreferredStartDate.HasValue ? a.PreferredStartDate.Value.Month : 0))
                    + 12 * ((a.PreferredEndDate.HasValue ? a.PreferredEndDate.Value.Year : 0) 
                    - (a.PreferredStartDate.HasValue ? a.PreferredStartDate.Value.Year : 0)) == PeriodMonths);
                }
                query = SortOrder switch
                {
                    "CreatedAt" => query.OrderBy(a => a.CreatedAt),
                    "CreatedAt_desc" => query.OrderByDescending(a => a.CreatedAt),
                    "Status" => query.OrderBy(a => a.Status),
                    "Status_desc" => query.OrderByDescending(a => a.Status),
                    _ => query.OrderBy(p => p.Title),

                };
                OpportunityRequests = await PaginatedList<TrainingOpportunityRequest>.CreateAsync(query, PageSize, PageIndex);
                return Page();

            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }
        }
    }
}

