using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.University.Specialties
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class DetailsModel : PageModel
    {
        private readonly SpecialtyRepository _specialtyRepository;

        public DetailsModel(
            SpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        public Specialty Specialty { get; set; }

        public int StudentCount { get; set; }

        public int OpportunityCount { get; set; }

        public int RequestCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Specialty = await _specialtyRepository
                .GetDetailsAsync(id);

            if (Specialty == null)
                return NotFound();

            StudentCount = Specialty
                .Students
                .Count;

            OpportunityCount = Specialty
                .OpportunitySpecialties
                .Count;

            RequestCount = Specialty
                .RequestSpecialties
                .Count;

            return Page();
        }
    }
}