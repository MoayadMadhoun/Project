using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.Institution.TrainingOppertunities
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class DetailsModel : PageModel
    {
        private readonly TrainingOpportunityRepository _repository;

        public DetailsModel(
            TrainingOpportunityRepository repository)
        {
            _repository = repository;
        }

        public TrainingOpportunity? Opportunity { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Opportunity =
                await _repository.GetDetailsAsync(id);

            if (Opportunity == null)
                return NotFound();

            return Page();
        }
    }
}