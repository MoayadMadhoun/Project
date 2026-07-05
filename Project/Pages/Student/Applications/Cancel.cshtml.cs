using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Student.Applications
{
    [Authorize(Roles = "Student")]
    public class CancelModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public CancelModel( ApplicationDbContext context,  UserManager<AspNetUser> userManager){
        
            _context = context;
            _userManager = userManager;
        }

        public TrainingApplication Application { get; set; } = default!;
        public bool IsAgree { get; set;  }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var curentStudent = await _context.Students.FirstOrDefaultAsync(s => s.UserID == user.Id);

            Application = await _context.TrainingApplications
             .Include(x => x.TrainingOpportunity)
                 .ThenInclude(x => x.TrainingInstitution)
             .FirstOrDefaultAsync(x =>
                 x.StudentID == curentStudent.StudentID &&
                 x.OpportunityID == id);

            if (Application == null)
                return NotFound();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync (int id) {

            var user = await _userManager.GetUserAsync(User);
            var curentStudent = await _context.Students.FirstOrDefaultAsync(s => s.UserID == user.Id);

            if (!IsAgree) // false 
            {
                ModelState.AddModelError(nameof(IsAgree), " يجب الموافقة لإكمال الإلغاء ⚠️ ");
                return Page();
            }
            // true
            var application = _context.TrainingApplications.FirstOrDefault(x=>x.OpportunityID == id && x.StudentID== curentStudent.StudentID);
            if (application == null)
                return NotFound();
            application.Status = Models.TrainingApplication.ApplicationStatus.Withdrawn;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Student/AvaliabelOpportunities");
        }

    }
}
