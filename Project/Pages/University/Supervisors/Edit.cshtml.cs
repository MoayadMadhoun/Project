using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using Project.Pages.University.ViewModels;
using Project.Repositories;
using Project.Repostory;
using Project.ViewModel;

namespace Project.Pages.University.Supervisors
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _repUniversity;
        private readonly DepartmentRepository _repDeparrtment;

        public EditModel(ApplicationDbContext dbContext, UniversityRepository repUniversity, DepartmentRepository repDeparrtment)
        {
            _dbContext = dbContext;
            _repUniversity = repUniversity;
            _repDeparrtment = repDeparrtment;
        }

        [BindProperty]
        public SupervisorsVM SupervisorsVM { get; set; }

        public AspNetUser? SuperInfo {  get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            var supervisor = await _repUniversity.GetUniversitySuperVisorByUserId(id);
            SuperInfo = await _repUniversity.GetUniversitySuperVisorByUserId(id);
            

            if(supervisor == null)
            {
                ModelState.AddModelError(nameof(supervisor), "„‘—› «·Ã«„⁄… Â–« €Ì— „ÊÃÊœ ");
                return Page();
            }
            FullViewModel(supervisor, SupervisorsVM);

            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var supervisor = await _repUniversity.GetUniversitySuperVisorByUserId(id);

            if (supervisor == null)
            {
                ModelState.AddModelError(nameof(supervisor), "„‘—› «·Ã«„⁄… Â–« €Ì— „ÊÃÊœ ");
                return Page();
            }

            await FullSuperVisor(SupervisorsVM, supervisor);

            return Page();
        }


        private void FullViewModel(AspNetUser user, SupervisorsVM vM)
        {
            vM.Name = user.FullName;
            vM.Email = user.Email;
            vM.IsActive = user.IsActive;
            vM.PhoneNumber = user.PhoneNumber;
        }

        private async Task FullSuperVisor(SupervisorsVM vM,AspNetUser user)
        {
            user.FullName = vM.Name;
            user.Email = vM.Email;
            user.PhoneNumber = vM.PhoneNumber;
            user.IsActive = vM.IsActive;

            await _dbContext.SaveChangesAsync();
        }
    }
}
