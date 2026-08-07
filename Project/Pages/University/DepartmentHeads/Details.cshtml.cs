using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repostory;

namespace Project.Pages.University.DepartmentHeads
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _repUniversity;

        public DetailsModel(ApplicationDbContext dbContext,UniversityRepository repUniversity)
        {
            _dbContext = dbContext;
            _repUniversity = repUniversity;
        }
        public AspNetUser? DepartmentHead { get; set; }

        public int StudentNumber { get; set; }
        public int SPNumber { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            DepartmentHead = await _repUniversity.GetDpartmentHeadByUserId(id);
            if (DepartmentHead is null)
            {
                ModelState.AddModelError(nameof(DepartmentHead), "áÇ íæÌÏ ÑÆíÓ ÞÓã ");
                return Page();
            }
            StudentNumber = await _dbContext.Students.CountAsync(s => s.DepartmentID == DepartmentHead.RoleScope!.DepartmentID);
            SPNumber = await _dbContext.Specialties.CountAsync(s => s.DepartmentID == DepartmentHead.RoleScope!.DepartmentID);
            return Page();
        }
    }
}
