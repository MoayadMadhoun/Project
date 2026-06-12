using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;
using Project.Repository;

namespace Project.Pages.Institution.CurrentTraining
{
    [Authorize(Roles = "InstitutionTrainingOfficer,InstitutionSupervisor")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TrainingInstitutionRepository _institutionRepository;
        private readonly UserManager<AspNetUser> _userManager;

        public DetailsModel(
            ApplicationDbContext dbContext,
            TrainingInstitutionRepository institutionRepository,
            UserManager<AspNetUser> userManager)
        {
            _dbContext = dbContext;
            _institutionRepository = institutionRepository;
            _userManager = userManager;
        }

        public TrainingDetailsVM Details { get; set; } = null!;
        public TrainingInstitution? CurrentInstitution { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var scope = await _dbContext.AspNetRoleScopes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserID == user.Id && s.IsActive);

            if (scope?.InstitutionID == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var institutionId = scope.InstitutionID.Value;
            CurrentInstitution = await _institutionRepository.GetByIdAsync(institutionId);
            if (CurrentInstitution == null)
                return RedirectToPage("/Index");

            var opportunity = await _institutionRepository
                .GetTrainingOpportunityDetailsForInstitutionAsync(id, institutionId);

            if (opportunity == null)
                return NotFound();

            var hasActivePlacements = await _institutionRepository
                .HasActivePlacementsForInstitutionAsync(id, institutionId);
            if (!hasActivePlacements)
                return NotFound();

            var skillNames = await _institutionRepository.GetOpportunitySkillNamesAsync(id);
            Details = TrainingDetailsVM.FromOpportunity(opportunity, skillNames);

            return Page();
        }
    }
}
