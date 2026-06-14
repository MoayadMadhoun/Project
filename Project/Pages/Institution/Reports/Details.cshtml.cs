using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Institution.Reports
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentReport Report { get; set; }

        public async Task<IActionResult> OnGetAsync(int reportId)
        {
            Report = await _context.StudentReports
                .Include(r => r.Student)
                .Include(r => r.Placement)
                    .ThenInclude(p => p.TrainingOpportunity)
                .Include(r => r.UniversitySupervisor)
                .FirstOrDefaultAsync(r =>
                    r.ReportID == reportId);

            if (Report == null)
                return NotFound();

            return Page();
        }
    }
}