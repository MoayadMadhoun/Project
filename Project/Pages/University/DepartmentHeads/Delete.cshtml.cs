using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repostory;
using System.Threading.Tasks;

namespace Project.Pages.University.DepartmentHeads
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
        public AspNetUser? DepartmentHead { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            DepartmentHead = await  _repUniversity.GetDpartmentHeadByUserId(id);
            if (DepartmentHead is null)
            {
                ModelState.AddModelError(nameof(DepartmentHead), "·« ÌÊÃœ —∆Ì” ﬁ”„ ");
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            var departmentHead = await _dbContext.Users .FirstOrDefaultAsync(u => u.Id == id);

            if(departmentHead is null)
            {
                return Page();
            }
            departmentHead.IsActive = false;

            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessafe"] = " „  ⁄ÿÌ· —∆Ì” «·ﬁ”„ »‰Ã«Õ";
            return Page();
        }
    }
}
