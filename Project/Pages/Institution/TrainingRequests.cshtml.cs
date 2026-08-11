using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;
using Project.Models.Enums;
using Project.Services;

namespace Project.Pages.Institution
{
    public class TrainingRequestsModel : PageModel
    {
        private readonly TrainingInstitutionRepository _institutionRepo;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly INotificationService _notificationService;

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

        public TrainingRequestsModel(TrainingInstitutionRepository institutionRepo, UserManager<AspNetUser> userManager, ApplicationDbContext dbContext, INotificationService notificationService)
        {
            _institutionRepo = institutionRepo;
            _userManager = userManager;
            _dbContext = dbContext;
            _notificationService = notificationService;
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
                                 .Include(a => a.Student)
                                 .Include(a => a.TrainingOpportunity)
                                    .ThenInclude(o => o.TrainingInstitution)
                                 .FirstOrDefaultAsync(a => a.ApplicationID == ApproveApplicationId);


                    if (currentApplication != null) 
                    {
                        currentApplication.InstitutionDecision = TrainingApplication.Decision.Approved;

                        currentApplication.Status = TrainingApplication.ApplicationStatus.Placed;

                        currentApplication.InstitutionOfficerID = user.Id;

                        currentApplication.InstitutionReviewedAt = DateTime.UtcNow;

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
                        await _dbContext.SaveChangesAsync();

                        await _notificationService.NotifyStudentAsync(
                            currentApplication.Student.UserID,
                            "تم قبولك في التدريب",
                            $"تم قبول طلب التدريب الخاص بك لدى مؤسسة {currentApplication.TrainingOpportunity.TrainingInstitution.Name}.",
                            NotificationType.Success,
                            "/Student/MyApplications",
                            "fa-solid fa-circle-check",
                            user.Id,
                            currentApplication.ApplicationID.ToString(),
                            nameof(TrainingApplication));

                        if (!exists)
                        {
                            const string title = "بدأ التدريب الميداني";
                            var message = $"تم بدء التدريب الميداني الخاص بالطالب {currentApplication.Student.Name}.";
                            var supervisorIds = await _dbContext.AspNetRoleScopes
                                .Where(s => s.IsActive &&
                                    ((s.Role.Name == "UniversitySupervisor" && s.UniversityID == currentApplication.Student.UniversityID) ||
                                     (s.Role.Name == "InstitutionSupervisor" && s.InstitutionID == institutionID)))
                                .Select(s => s.UserID)
                                .Distinct()
                                .ToListAsync();

                            await _notificationService.NotifyStudentAsync(currentApplication.Student.UserID, title, message, NotificationType.Training, "/Student/CurrentTraining", "fa-solid fa-briefcase", user.Id, currentApplication.ApplicationID.ToString(), nameof(TrainingPlacement));
                            await _notificationService.NotifyUsersAsync(supervisorIds, title, message, NotificationType.Training, "/Institution/CurrentTraining", "fa-solid fa-briefcase", user.Id, currentApplication.ApplicationID.ToString(), nameof(TrainingPlacement));
                            await _notificationService.NotifyInstitutionTrainingOfficersAsync(institutionID, title, message, NotificationType.Training, "/Institution/CurrentTraining", "fa-solid fa-briefcase", user.Id, currentApplication.ApplicationID.ToString(), nameof(TrainingPlacement));
                        }
                    }

                    if (currentApplication.InstitutionDecision == TrainingApplication.Decision.Approved)
                    {
                        return RedirectToPage();
                    }
                }
                if (RejectApplicationId > 0)
                {
                    var currentApplication = await _dbContext.TrainingApplications
                        .Include(a => a.Student)
                        .FirstOrDefaultAsync(a => a.ApplicationID == RejectApplicationId);
                    if (currentApplication != null)
                    {
                        currentApplication.InstitutionDecision = TrainingApplication.Decision.Rejected;
                        currentApplication.Status = TrainingApplication.ApplicationStatus.InstitutionRejected;
                        currentApplication.InstitutionOfficerID = user.Id;
                        currentApplication.InstitutionReviewedAt = DateTime.UtcNow;
                        _dbContext.TrainingApplications.Update(currentApplication);
                        await _dbContext.SaveChangesAsync();
                        await _notificationService.NotifyStudentAsync(
                            currentApplication.Student.UserID,
                            "لم يتم قبول طلب التدريب",
                            "نأسف، لم يتم قبول طلب التدريب من قبل المؤسسة.",
                            NotificationType.Error,
                            "/Student/MyApplications",
                            "fa-solid fa-circle-xmark",
                            user.Id,
                            currentApplication.ApplicationID.ToString(),
                            nameof(TrainingApplication));
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
