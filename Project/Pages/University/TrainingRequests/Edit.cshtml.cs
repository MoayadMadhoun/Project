using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repository;
using Project.Repostory;
using Project.Services;

namespace Project.Pages.University.TrainingRequests
{
    [Authorize(Roles = "UniversityTrainingAdmin, DepartmentHead")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly TrainingInstitutionRepository _institutionRepository;
        private readonly TrainingRequestService _trainingRequestService;

        public EditModel(
            ApplicationDbContext dbContext,
            UserManager<AspNetUser> userManager,
            TrainingInstitutionRepository institutionRepository,
            TrainingRequestService trainingRequestService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _institutionRepository = institutionRepository;
            _trainingRequestService = trainingRequestService;
        }

        public TrainingOpportunityRequest? OpportunityRequest { get; set; }
        public SelectList TrainingTermsList { get; set; }
        public SelectList InstitutionsList { get; set; }
        public List<SelectListItem> SpecialtiesList { get; set; } = new();

        [BindProperty]
        public EditTrainingRequestVM TrainingRequest { get; set; }

        public async Task<IActionResult> OnGet([FromRoute] int RequestId)
        {
            await ReloadDropdownsAsync();

            OpportunityRequest = _dbContext.TrainingOpportunityRequests
                .FirstOrDefault(tr => tr.RequestID == RequestId);

            if (OpportunityRequest == null)
                return NotFound();

            var selectedSpecialties = _dbContext.RequestSpecialties
                .Where(rs => rs.RequestID == RequestId)
                .Select(rs => rs.SpecialtyID)
                .ToList();

            var selectedSkills = _dbContext.RequestSkills
                .Where(rs => rs.RequestID == RequestId)
                .ToList();

            var allSkills = await _dbContext.Skills
                .OrderBy(s => s.Name)
                .ToListAsync();


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
                Status = OpportunityRequest.Status,
                SelectedSpecialties = selectedSpecialties,
                //InstitutionID = selectedInstition.InstituationID,

                Skills = allSkills.Select(s =>
                {
                    var requestSkill = selectedSkills
                        .FirstOrDefault(rs => rs.SkillID == s.SkillID);

                    return new RequestSkillVM
                    {
                        SkillId = s.SkillID,
                        SkillName = s.Name,
                        Selected = requestSkill != null,
                        IsRequired = requestSkill?.IsRequired ?? true
                    };
                }).ToList()
            
            };

            var requestInstitution = await _dbContext.OpportunityRequestInstitutions
                        .FirstOrDefaultAsync(x => x.RequestID == RequestId);

            if (requestInstitution != null)
            {
                TrainingRequest.InstitutionID = requestInstitution.InstitutionID;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost([FromRoute] int RequestId)
        {
            if (!ModelState.IsValid)
            {
                await ReloadDropdownsAsync();
                OpportunityRequest = _dbContext.TrainingOpportunityRequests
                    .FirstOrDefault(tr => tr.RequestID == RequestId);
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            var scope = _dbContext.AspNetRoleScopes
                .FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

            if (scope?.UniversityID == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var success = await _trainingRequestService
                .UpdateTrainingRequestAsync(RequestId, TrainingRequest, user, scope);

            if (!success)
                return RedirectToPage("/Index");

            return RedirectToPage("/University/AvailabelOpportunities");
        }

        private async Task ReloadDropdownsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var scope = _dbContext.AspNetRoleScopes
                .FirstOrDefault(x => x.UserID == user.Id && x.IsActive);

            TrainingTermsList = new SelectList(
                _dbContext.TrainingTerms,
                nameof(TrainingTerm.TermID),
                nameof(TrainingTerm.Name));

            InstitutionsList = new SelectList(
                await _institutionRepository.GetAllAsync(),
                nameof(TrainingInstitution.InstituationID),
                nameof(TrainingInstitution.Name));

            if (scope?.UniversityID != null)
            {
                SpecialtiesList = await _dbContext.Specialties
                    .Where(x => x.Department.UniversityID == scope.UniversityID)
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem
                    {
                        Value = x.SpecialtyID.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync();
            }
        }
    }
}