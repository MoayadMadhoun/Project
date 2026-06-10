using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using Project.Repostory;

namespace Project.Pages.University.Supervisors
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _repUniversity;

        public DeleteModel(ApplicationDbContext dbContext,UniversityRepository repUniversity)
        {
            _dbContext = dbContext;
            _repUniversity = repUniversity;
        }

        public AspNetUser? Supervisor { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            Supervisor = await _repUniversity.GetUniversitySuperVisorByUserId(id);

            if(Supervisor == null)
            {
                ModelState.AddModelError(nameof(Supervisor), "·« ÌÊÃœ —∆Ì” Ã«„⁄… ");

                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            var UniversitySupervisor = await _repUniversity.GetUniversitySuperVisorByUserId(id);

            if (Supervisor == null)
            {
                ModelState.AddModelError(nameof(Supervisor), "·« ÌÊÃœ —∆Ì” Ã«„⁄… ");
                return Page();
            }
            UniversitySupervisor?.IsActive = false;
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessafe"] = " „  ⁄ÿÌ· —∆Ì” «·Ã«„⁄… »‰Ã«Õ";

            return Page();
        }

    }
}
