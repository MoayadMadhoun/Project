using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.Institution.TrainingOppertunities
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class DeleteModel : PageModel
    {
        private readonly TrainingOpportunityRepository _repository;

        public DeleteModel(
            TrainingOpportunityRepository repository)
        {
            _repository = repository;
        }

        public TrainingOpportunity? Opportunity { get; set; }

        [BindProperty]
        public int OpportunityId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Opportunity =
                await _repository.GetDetailsAsync(id);

            if (Opportunity == null)
                return NotFound();

            OpportunityId = id;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result =
                await _repository.DeleteSoftAsync(
                    OpportunityId);

            if (!result)
                return NotFound();

            TempData["SuccessMessage"] =
                "تم إلغاء الفرصة التدريبية بنجاح";

            return RedirectToPage("Index");
        }
    }
}