using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Services;

namespace Project.Pages.CommonPages.TrainingTerm
{
    public class CreateModel : PageModel
    {
        private readonly TrainingTermService _trainingTermService;

        public CreateModel(TrainingTermService trainingTermService)
        {
            _trainingTermService = trainingTermService;
        }

        [BindProperty]
        public Models.TrainingTerm Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _trainingTermService.CreateAsync(Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }

            TempData["Success"] = result.Message;

            return RedirectToPage("Index");
        }
    }
}
