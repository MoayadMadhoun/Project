using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Institution.Attendance
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public AttendanceRecord Attendance { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Attendance = await _context.AttendanceRecords
                .Include(a => a.TrainingPlacement)
                    .ThenInclude(p => p.Student)
                .FirstOrDefaultAsync(a => a.AttendanceID == id);

            if (Attendance == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var attendance = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.AttendanceID == id);

            if (attendance == null)
                return NotFound();

            _context.AttendanceRecords.Remove(attendance);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف سجل الحضور بنجاح";

            return RedirectToPage("/Institution/Attendance");
        }
    }
}