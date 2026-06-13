using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repository;
using Project.Repostory;
using System.ComponentModel.DataAnnotations;
using static Project.Models.TrainingOpportunityRequest;

namespace Project.Pages.University.TrainingRequests
{
    [Authorize(Roles = "UniversityTrainingAdmin, DepartmentHead")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly UniversityRepository _universityRepo;
        private readonly TrainingInstitutionRepository _institutionRepository;


        public CreateModel(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, UniversityRepository universityRepo, TrainingInstitutionRepository institutionRepository )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _universityRepo = universityRepo;
            _institutionRepository = institutionRepository;

        }
        [BindProperty]
        public TrainingRequestVM TrainingRequest { get; set; }
        public SelectList TrainingTermsList { get; set; }
        public SelectList InstitutionsList { get; set; }

        public OpportunityRequestInstitution RequestInstitution { get; set; }
        public TrainingOpportunityRequest OpportunityRequest { get; set; } = new TrainingOpportunityRequest();
        public Models.University? CurrentUniversity { get; set; }

       
        public async void OnGet()
        {
            var query = _dbContext.TrainingTerms.ToList();
            TrainingTermsList = new SelectList(query, nameof(TrainingTerm.TermID), nameof(TrainingTerm.Name));
            InstitutionsList = new SelectList(await _institutionRepository.GetAllAsync(), nameof(TrainingInstitution.InstituationID), nameof(TrainingInstitution.Name));
        }
        public async Task<IActionResult> OnPost()
        {
            if (TrainingRequest.PreferredStartDate != default &&
            TrainingRequest.PreferredStartDate < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(TrainingRequest.PreferredStartDate),
                    "لا يمكن اختيار تاريخ بداية في الماضي");
            }

            if (TrainingRequest.PreferredEndDate != default &&
                TrainingRequest.PreferredEndDate < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(TrainingRequest.PreferredEndDate),
                    "لا يمكن اختيار تاريخ نهاية في الماضي");
            }

            if (TrainingRequest.PreferredEndDate <= TrainingRequest.PreferredStartDate)
            {
                ModelState.AddModelError(
                    nameof(TrainingRequest.PreferredEndDate),
                    "يجب أن يكون تاريخ النهاية بعد تاريخ البداية");
            }
            if (!ModelState.IsValid)
            {
                var query = _dbContext.TrainingTerms.ToList();
                TrainingTermsList = new SelectList(query, nameof(TrainingTerm.TermID), nameof(TrainingTerm.Name));
                return Page();
            }

            AspNetUser? user = await _userManager.GetUserAsync(User);
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
            OpportunityRequest = new TrainingOpportunityRequest
            {
                Title = TrainingRequest.Title,
                Description = TrainingRequest.Description,
                PreferredEndDate = TrainingRequest.PreferredEndDate,
                PreferredStartDate= TrainingRequest.PreferredStartDate,
                RequestedSeats= TrainingRequest.RequestedSeats,
                Notes= TrainingRequest.Notes,
                TermID= TrainingRequest.TermID,
                Status= TrainingRequest.Status,
                UniversityID = CurrentUniversity.UniversityID,
            };
            if (User.IsInRole("UniversityTrainingAdmin") && user != null)
            {
                OpportunityRequest.UniversityAdminID = user.Id  ;
                OpportunityRequest.DepartmentHeadID = null;
            }
            else if(User.IsInRole("DepartmentHead") && user != null)
            {
                OpportunityRequest.DepartmentHeadID = user.Id;
                OpportunityRequest  .UniversityAdminID = null;
            }
           
           await  _dbContext.TrainingOpportunityRequests.AddAsync(OpportunityRequest);
           await _dbContext.SaveChangesAsync();
            if(OpportunityRequest.Status == TrainingOpportunityRequest.RequestStatus.Published)
            {
                RequestInstitution = new OpportunityRequestInstitution
                {
                    RequestID = OpportunityRequest.RequestID,
                    InstitutionID = TrainingRequest.InstitutionID
                };
                await _dbContext.OpportunityRequestInstitutions.AddAsync(RequestInstitution);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToPage("/University/AvailabelOpportunities");
        }
    }
}
