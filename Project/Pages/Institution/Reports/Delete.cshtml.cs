using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Institution.Reports
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentReport Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int reportId)
        {
            Report = await _context.StudentReports
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r =>
                    r.ReportID == reportId);

            if (Report == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int reportId)
        {
            var report = await _context.StudentReports
                .FirstOrDefaultAsync(r =>
                    r.ReportID == reportId);

            if (report == null)
                return NotFound();

            _context.StudentReports.Remove(report);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "تم حذف التقرير بنجاح";

            return RedirectToPage(
                "/Institution/Reports");
        }
    }
}