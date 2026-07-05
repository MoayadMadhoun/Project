using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;

namespace Project.Pages.Institution.Attendance;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AspNetUser> _userManager;

    public EditModel(
        ApplicationDbContext context,
        UserManager<AspNetUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public EditAttendanceVM Input { get; set; } = new();

    public string StudentName { get; set; } = "";
    public string OpportunityTitle { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(int attendanceId)
    {
        var attendance = await _context.AttendanceRecords
            .Include(x => x.TrainingPlacement)
                .ThenInclude(x => x.Student)
            .Include(x => x.TrainingPlacement)
                .ThenInclude(x => x.TrainingOpportunity)
            .FirstOrDefaultAsync(x => x.AttendanceID == attendanceId);

        if (attendance == null)
            return NotFound();

        Input = new EditAttendanceVM
        {
            AttendanceID = attendance.AttendanceID,
            PlacementID = attendance.PlacementID,
            AttendanceDate = attendance.AttendanceDate,
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            Status = attendance.Status,
            Notes = attendance.Notes
        };

        StudentName = attendance.TrainingPlacement.Student.Name;
        OpportunityTitle = attendance.TrainingPlacement.TrainingOpportunity.Title;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var attendance = await _context.AttendanceRecords
            .FirstOrDefaultAsync(x => x.AttendanceID == Input.AttendanceID);

        if (attendance == null)
            return NotFound();

        attendance.AttendanceDate = Input.AttendanceDate;
        attendance.CheckInTime = Input.CheckInTime;
        attendance.CheckOutTime = Input.CheckOutTime;
        attendance.Status = Input.Status;
        attendance.Notes = Input.Notes;

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تعديل سجل الحضور بنجاح";

        return RedirectToPage(
            "/Institution/Attendance",
            new
            {
                placementId = attendance.PlacementID
            });
    }
}