using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.University.Specialties
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly SpecialtyRepository _specialtyRepository;

        public DeleteModel(
            SpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        public Specialty Specialty { get; set; }

        [BindProperty]
        public int SpecialtyID { get; set; }

        public int StudentCount { get; set; }

        public int OpportunityCount { get; set; }

        public int RequestCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Specialty = await _specialtyRepository
                .GetDetailsAsync(id);

            if (Specialty == null)
                return NotFound();

            SpecialtyID = Specialty.SpecialtyID;

            StudentCount = Specialty.Students.Count;

            OpportunityCount = Specialty
                .OpportunitySpecialties
                .Count;

            RequestCount = Specialty
                .RequestSpecialties
                .Count;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _specialtyRepository
                .DeleteSoftAsync(SpecialtyID);

            if (!result)
            {
                TempData["ErrorMessage"] =
                    "التخصص غير موجود أو تم تعطيله مسبقاً";

                return RedirectToPage("Index");
            }

            TempData["SuccessMessage"] =
                "تم تعطيل التخصص بنجاح";

            return RedirectToPage("Index");
        }
    }
}