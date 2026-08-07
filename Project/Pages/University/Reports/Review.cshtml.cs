using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.University.Reports
{
    public class ReviewModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReviewModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentReport Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int reportId)
        {
            Report = await _context.StudentReports

                .Include(r => r.Student)

                .Include(r => r.Placement)
                    .ThenInclude(p => p.TrainingInstitution)

                .Include(r => r.Placement)
                    .ThenInclude(p => p.TrainingOpportunity)

                .FirstOrDefaultAsync(r =>
                    r.ReportID == reportId);

            if (Report == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var report = await _context.StudentReports
                .FirstOrDefaultAsync(r =>
                    r.ReportID == Report.ReportID);

            if (report == null)
                return NotFound();

            report.Status = Report.Status;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "تم تحديث حالة التقرير بنجاح";

            return RedirectToPage("/University/Reports/Index");
        }
    }
}