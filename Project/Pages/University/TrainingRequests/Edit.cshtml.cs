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
        public EditTrainingRequestVM TrainingRequest { get; set; }
      

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
            TrainingRequest = new EditTrainingRequestVM
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
