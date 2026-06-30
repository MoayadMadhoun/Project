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
using System.ComponentModel.DataAnnotations;
using static Project.Models.OpportunityRequestInstitution;
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
        private readonly TrainingRequestService _trainingRequestService;

        public CreateModel(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, UniversityRepository universityRepo, TrainingInstitutionRepository institutionRepository, TrainingRequestService trainingRequestService )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _universityRepo = universityRepo;
            _institutionRepository = institutionRepository;
            _trainingRequestService = trainingRequestService;
        }
        [BindProperty]
        public TrainingRequestVM TrainingRequest { get; set; }
        public SelectList TrainingTermsList { get; set; }
        public SelectList InstitutionsList { get; set; }

        public OpportunityRequestInstitution RequestInstitution { get; set; }
        public TrainingOpportunityRequest OpportunityRequest { get; set; } = new TrainingOpportunityRequest();
        public OpportunityRequestInstitution opportunityRequestInstitution { get; set; }
        public Models.University? CurrentUniversity { get; set; }
        public List<SelectListItem> SpecialtiesList { get; set; } = new();
        //public List<Skill> SkillsList { get; set; } = new();

        public async Task OnGet()
        {
            TrainingRequest = new TrainingRequestVM();
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
                    .Where(x =>
                        x.Department.UniversityID ==
                        scope.UniversityID)
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem
                    {
                        Value = x.SpecialtyID.ToString(),
                        Text = x.Name
                    }).ToListAsync();
            }

            TrainingRequest.Skills = await _dbContext.Skills
                 .OrderBy(x => x.Name)
                 .Select(s => new RequestSkillVM
                 {
                     SkillId = s.SkillID,
                     SkillName = s.Name,
                     Selected = false,
                     IsRequired = true
                 })
                 .ToListAsync();


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
            AspNetUser? user = await _userManager.GetUserAsync(User);
            AspNetRoleScope? scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

            if (!ModelState.IsValid)
            {
                var query = _dbContext.TrainingTerms.ToList();
                TrainingTermsList = new SelectList(
                    _dbContext.TrainingTerms,
                    nameof(TrainingTerm.TermID),
                    nameof(TrainingTerm.Name));

                InstitutionsList = new SelectList(
                    await _institutionRepository.GetAllAsync(),
                    nameof(TrainingInstitution.InstituationID),
                    nameof(TrainingInstitution.Name));

                SpecialtiesList = await _dbContext.Specialties
                    .Where(x => x.Department.UniversityID == scope.UniversityID)
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem
                    {
                        Value = x.SpecialtyID.ToString(),
                        Text = x.Name
                    })
                    .ToListAsync();

                TrainingRequest.Skills = await _dbContext.Skills
                    .OrderBy(x => x.Name)
                    .Select(s => new RequestSkillVM
                    {
                        SkillId = s.SkillID,
                        SkillName = s.Name,
                        Selected = false,
                        IsRequired = true
                    })
                    .ToListAsync();

                return Page();
            }

           
            if (scope == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (scope.UniversityID == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            await _trainingRequestService.CreateTrainingRequestAsync(TrainingRequest,user, scope, User.IsInRole("UniversityTrainingAdmin"));

            return RedirectToPage("/University/AvailabelOpportunities");
        }
    }
}
