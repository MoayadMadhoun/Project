using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repostory;

namespace Project.Pages.University.Supervisors
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _repUniversity;

        public DeleteModel(
            ApplicationDbContext dbContext,
            UniversityRepository repUniversity)
        {
            _dbContext = dbContext;
            _repUniversity = repUniversity;
        }

        public SupervisorsVM Supervisor { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _repUniversity.GetUniversitySuperVisorByUserId(id);

            if (user == null)
                return NotFound();

            Supervisor = new SupervisorsVM
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,

                //UniversityName = user.University.Name ?? null
            };

            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _repUniversity.GetUniversitySuperVisorByUserId(id);

            if (user == null)
                return NotFound();

            user.IsActive = false;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = " „  ⁄ÿÌ· „‘—› «·Ã«„⁄… »‰Ã«Õ";

            return RedirectToPage("/University/Supervisors");
        }
        

    }
}
