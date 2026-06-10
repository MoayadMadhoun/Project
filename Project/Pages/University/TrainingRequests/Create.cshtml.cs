using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Data;
using Project.Models;
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
        public RequestInput TrainingRequest { get; set; }
        public SelectList TrainingTermsList { get; set; }
        public SelectList InstitutionsList { get; set; }

        public OpportunityRequestInstitution RequestInstitution { get; set; }
        public TrainingOpportunityRequest OpportunityRequest { get; set; } = new TrainingOpportunityRequest();
        public Models.University? CurrentUniversity { get; set; }

        public class RequestInput
        {
            [Required(ErrorMessage = "Title is required")]
            [MaxLength(300, ErrorMessage = "Title can't be more than 200 characters")]
            [MinLength(3, ErrorMessage = "Title can't be less than 3 characters")]
            public string Title { get; set; } = string.Empty;
            [MaxLength(1000, ErrorMessage = "Description can't be more than 1000 characters")]
            public string? Description { get; set; }
            [Required(ErrorMessage = "Requested seats number is required")]

            public int RequestedSeats { get; set; }
            public int InstitutionID { get; set; }
            public DateTime? PreferredStartDate { get; set; }
            public DateTime? PreferredEndDate { get; set; }

            public DateTime? ApplicationDeadline { get; set; }
            [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
            public string? Notes { get; set; }

            public int TermID { get; set; }
            public RequestStatus Status { get; set; } = RequestStatus.Draft;

        }
        public async void OnGet()
        {
            var query = _dbContext.TrainingTerms.ToList();
            TrainingTermsList = new SelectList(query, nameof(TrainingTerm.TermID), nameof(TrainingTerm.Name));
            InstitutionsList = new SelectList(await _institutionRepository.GetAllAsync(), nameof(TrainingInstitution.InstituationID), nameof(TrainingInstitution.Name));
        }
        public async Task<IActionResult> OnPost()
        {
            
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
