using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.University
{
    public class StudentDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StudentDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Student Student { get; set; }

        public TrainingPlacement? Placement { get; set; }

        public int AttendanceCount { get; set; }
        public int EvaluationCount { get; set; }
        public int ReportCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Student = await _context.Students

                .Include(s => s.Department)
                .Include(s => s.Specialty)
                .Include(s => s.University)
                .Include(s => s.User)

                .FirstOrDefaultAsync(s =>
                    s.StudentID == id);

            if (Student == null)
                return NotFound();

            Placement = await _context.TrainingPlacements

                .Include(p => p.TrainingOpportunity)
                .Include(p => p.TrainingInstitution)

                .FirstOrDefaultAsync(p =>
                    p.StudentID == id);

            AttendanceCount = await _context.AttendanceRecords
                .CountAsync(a =>
                    a.TrainingPlacement.StudentID == id);

            EvaluationCount = await _context.StudentEvaluations
                .CountAsync(e =>
                    e.TrainingPlacement.StudentID == id);

            ReportCount = await _context.StudentReports
                .CountAsync(r =>
                    r.StudentID == id);

            return Page();
        }
    }
}