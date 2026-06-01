using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;
using Project.Repostory;

namespace Project.Pages.Institution
{
    public class AvailableOpportunitiesModel : PageModel
    {
        private readonly TrainingInstitutionRepository _institutionRepo;
        private readonly ApplicationDbContext _dbContext;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        public TrainingInstitution CurrentInstitution { get; set; }
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

        public PaginatedList<TrainingOpportunity> TrainingOpportunities { get; set; }
        public AvailableOpportunitiesModel(TrainingInstitutionRepository institutionRepo, ApplicationDbContext dbContext, UserManager<AspNetUser> userManager)
        {
            _institutionRepo = institutionRepo;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<ActionResult> OnGet()
        {
            try
            {
                if (User == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {

                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                //int InstituationID = scope.InstitutionID;

                if (scope.InstitutionID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                int institutionId = scope.InstitutionID.Value;

                CurrentInstitution = await _institutionRepo.GetByIdAsync(institutionId);
                if (CurrentInstitution == null)
                {
                    return RedirectToPage("/Index");
                }
                
                var query = _institutionRepo.GetTrainingOpportunitiesQueryable(institutionId);
                var specialities = _dbContext.Specialties.ToList();
                Specialities = new SelectList(specialities, "SpecialtyID", "Name");
                if (SpecialtyId > 0)
                    query = query.Where(a => a.Specialties.Any(s => s.SpecialtyID == SpecialtyId));
                if (StatusId > 0)
                {
                    query = query.Where(a => a.Status == (TrainingOpportunity.Opportunity)StatusId);
                }
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.Title.Contains(SearchTerm));
                }
                if(PeriodMonths > 0)
                {
                    query = query.Where(a => (a.EndDate.Month-a.StartDate.Month)+12*(a.EndDate.Year-a.StartDate.Year) == PeriodMonths);
                }
                query = SortOrder switch
                {
                    "CreatedAt" => query.OrderBy(a => a.CreatedAt),
                    "CreatedAt_desc" => query.OrderByDescending(a => a.CreatedAt),
                    "Capacity" => query.OrderBy(a => a.Capacity),
                    "Capacity_desc" => query.OrderByDescending(a => a.Capacity),
                    "Status" => query.OrderBy(a => a.Status),
                    "Status_desc" => query.OrderByDescending(a => a.Status),
                    _ => query.OrderBy(p => p.Title),

                };
                TrainingOpportunities = await PaginatedList<TrainingOpportunity>.CreateAsync(query, PageSize, PageIndex);
                return Page();

            }
            catch (Exception ex) 
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
