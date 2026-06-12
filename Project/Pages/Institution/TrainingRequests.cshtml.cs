using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;

namespace Project.Pages.Institution
{
    public class TrainingRequestsModel : PageModel
    {
        private readonly TrainingInstitutionRepository _institutionRepo;
        private readonly UserManager<AspNetUser> _userManager;
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
        public int ApplicationStatusId { get; set; }
        public PaginatedList<TrainingApplication> TrainingRequests { get; set; }
        public TrainingInstitution CurrentInstitution { get; set; }
        [BindProperty(SupportsGet = true)]
        public int ApproveApplicationId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int RejectApplicationId { get; set; }

        public TrainingRequestsModel(TrainingInstitutionRepository institutionRepo, UserManager<AspNetUser> userManager, ApplicationDbContext dbContext)
        {
            _institutionRepo = institutionRepo;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
               
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


                int institutionID = scope.InstitutionID.GetValueOrDefault();
                if (institutionID == 0)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                CurrentInstitution = await _institutionRepo.GetByIdAsync(institutionID);
                if (CurrentUniversity == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                var query = _institutionRepo.GetApplicationsForInstitution(institutionID).Where(tr => tr.Status == TrainingApplication.ApplicationStatus.UniversityApproved);
               

                if (ApproveApplicationId > 0)
                {
                    var currentApplication = await _dbContext.TrainingApplications
                                 .Include(a => a.TrainingOpportunity)
                                 .FirstOrDefaultAsync(a => a.ApplicationID == ApproveApplicationId);


                    if (currentApplication != null) 
                    {
                        currentApplication.InstitutionDecision = TrainingApplication.Decision.Approved;

                        currentApplication.Status = TrainingApplication.ApplicationStatus.Placed;

                        currentApplication.InstitutionOfficerID = user.Id;

                        currentApplication.InstitutionReviewedAt = DateTime.Now;

                        var exists = await _dbContext.TrainingPlacements
                             .AnyAsync(p => p.ApplicationID == currentApplication.ApplicationID);

                        if (!exists)
                        {
                            var placement = new TrainingPlacement
                            {
                                OpportunityID = currentApplication.OpportunityID,
                                ApplicationID = currentApplication.ApplicationID,
                                StudentID = currentApplication.StudentID,
                                InstitutionID = institutionID,
                                TermID = currentApplication.TrainingOpportunity.TermID,
                                StartDate = currentApplication.TrainingOpportunity.StartDate,
                                EndDate = currentApplication.TrainingOpportunity.EndDate,
                                Status = TrainingPlacement.PlacementStatus.InProgress
                            };

                            _dbContext.TrainingPlacements.Add(placement);
                        }

                        _dbContext.TrainingApplications.Update(currentApplication);
                        _dbContext.SaveChanges();
                    }

                    if (currentApplication.InstitutionDecision == TrainingApplication.Decision.Approved)
                    {
                        return RedirectToPage();
                    }
                }
                if (RejectApplicationId > 0)
                {
                    var currentApplication = _dbContext.TrainingApplications.FirstOrDefault(a => a.ApplicationID == RejectApplicationId);
                    if (currentApplication != null)
                    {
                        currentApplication.InstitutionDecision = TrainingApplication.Decision.Rejected;
                        currentApplication.Status = TrainingApplication.ApplicationStatus.InstitutionRejected;
                        _dbContext.TrainingApplications.Update(currentApplication);
                        _dbContext.SaveChanges();
                    }
                }



                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.Student.Name.Contains(SearchTerm));
                }
                query = ApplicationStatusId switch
                {
                    0 => query.Where(tr => tr.InstitutionDecision == TrainingApplication.Decision.Pending),
                    1 => query.Where(tr => tr.InstitutionDecision == TrainingApplication.Decision.Rejected),
                    2 => query.Where(tr => tr.InstitutionDecision == TrainingApplication.Decision.Approved),
                    _ => query
                };
                query = SortOrder switch
                {
                    "Name" => query.OrderBy(a => a.Student.Name),
                    "Name_desc" => query.OrderByDescending(a => a.Student.Name),
                    _ => query.OrderBy(p => p.AppliedAt),

                };
                TrainingRequests = await PaginatedList<TrainingApplication>.CreateAsync(query, PageSize, PageIndex);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }
        }
    }
}