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
    public class EditModel : PageModel
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly UniversityRepository _universityRepo;
        private readonly TrainingInstitutionRepository _institutionRepository;

        public Models.University? CurrentUniversity { get; set; }
        public EditModel(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, UniversityRepository universityRepo, TrainingInstitutionRepository institutionRepository)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _universityRepo = universityRepo;
            _institutionRepository = institutionRepository;

        }
        public TrainingOpportunityRequest? OpportunityRequest { get; set; } = new TrainingOpportunityRequest();
        public SelectList TrainingTermsList { get; set; }
        public SelectList InstitutionsList { get; set; }
        [BindProperty]
        public RequestInput TrainingRequest { get; set; }
        public class RequestInput
        {
            [Required(ErrorMessage = "Title is required")]
            [MaxLength(300, ErrorMessage = "Title can't be more than 200 characters")]
            [MinLength(3, ErrorMessage = "Title can't be less than 3 characters")]
            public string Title { get; set; } = string.Empty;
            [MaxLength(1000, ErrorMessage = "Description can't be more than 1000 characters")]
            public string? Description { get; set; }
            [Required(ErrorMessage = "Requested seats number is required")]
            [Range(0, 200, ErrorMessage = "Seats can't be less than zero")]
            public int RequestedSeats { get; set; }
            public DateTime? PreferredStartDate { get; set; }
            public DateTime? PreferredEndDate { get; set; }

            public DateTime? ApplicationDeadline { get; set; }
            [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
            public string? Notes { get; set; }

            public int TermID { get; set; }
            public RequestStatus Status { get; set; } = RequestStatus.Draft;

        }

        public async Task<IActionResult> OnGet([FromRoute] int RequestId)
        {
            var query = _dbContext.TrainingTerms.ToList();
            TrainingTermsList = new SelectList(query, nameof(TrainingTerm.TermID), nameof(TrainingTerm.Name));
            InstitutionsList = new SelectList(await _institutionRepository.GetAllAsync(), nameof(TrainingInstitution.InstituationID), nameof(TrainingInstitution.Name));
            OpportunityRequest = _dbContext.TrainingOpportunityRequests.Where(tr => tr.RequestID == RequestId).FirstOrDefault();
            if (OpportunityRequest == null)
            {
                return NotFound();
            }
            TrainingRequest = new RequestInput
            {
                Title = OpportunityRequest.Title,
                Description = OpportunityRequest.Description,
                PreferredEndDate = OpportunityRequest.PreferredEndDate,
                PreferredStartDate = OpportunityRequest.PreferredStartDate,
                RequestedSeats = OpportunityRequest.RequestedSeats,
                Notes = OpportunityRequest.Notes,
                TermID = OpportunityRequest.TermID,
                ApplicationDeadline = OpportunityRequest.ApplicationDeadline,
                Status = OpportunityRequest.Status

            };
            return Page();
        }
        public async Task<IActionResult> OnPost([FromRoute] int RequestId)
        {

            if (!ModelState.IsValid)
            {
                var query = _dbContext.TrainingTerms.ToList();
                TrainingTermsList = new SelectList(query, nameof(TrainingTerm.TermID), nameof(TrainingTerm.Name));
                OpportunityRequest = _dbContext.TrainingOpportunityRequests.Where(tr => tr.RequestID == RequestId).FirstOrDefault();
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
            OpportunityRequest = _dbContext.TrainingOpportunityRequests.Where(tr => tr.RequestID == RequestId).FirstOrDefault();
            if (CurrentUniversity.UniversityID != OpportunityRequest?.UniversityID)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            if (OpportunityRequest != null)
            {

                OpportunityRequest.Title = TrainingRequest.Title;
                OpportunityRequest.Description = TrainingRequest.Description;
                OpportunityRequest.PreferredEndDate = TrainingRequest.PreferredEndDate;
                OpportunityRequest.PreferredStartDate = TrainingRequest.PreferredStartDate;
                OpportunityRequest.RequestedSeats = TrainingRequest.RequestedSeats;
                OpportunityRequest.ApplicationDeadline = TrainingRequest.ApplicationDeadline;
                OpportunityRequest.Notes = TrainingRequest.Notes;
                OpportunityRequest.TermID = TrainingRequest.TermID;
                OpportunityRequest.Status = TrainingRequest.Status;
                _dbContext.TrainingOpportunityRequests.Update(OpportunityRequest);
                await _dbContext.SaveChangesAsync();
            }



            return RedirectToPage("/University/AvailabelOpportunities");
        }
    }
}
