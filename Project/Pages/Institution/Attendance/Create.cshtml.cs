using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;

namespace Project.Pages.Institution.Attendance;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AspNetUser> _userManager;

    public CreateModel(
        ApplicationDbContext context,
        UserManager<AspNetUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public CreateAttendanceVM Input { get; set; } = new();

    [TempData]
    public string ErrorMessage { get; set; }

    public string StudentName { get; set; } = "";
    public string OpportunityTitle { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(int placementId)
    {
        var placement = await _context.TrainingPlacements
            .Include(x => x.Student)
            .Include(x => x.TrainingOpportunity)
            .FirstOrDefaultAsync(x => x.PlacementID == placementId);

        if (placement == null)
            return NotFound();

        Input.PlacementID = placementId;

        StudentName = placement.Student.Name;
        OpportunityTitle = placement.TrainingOpportunity.Title;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var placement = await _context.TrainingPlacements
            .FirstOrDefaultAsync(x => x.PlacementID == Input.PlacementID);

        if (placement == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        var attendance = new AttendanceRecord
        {
            PlacementID = placement.PlacementID,
            AttendanceDate = Input.AttendanceDate,
            CheckInTime = Input.CheckInTime,
            CheckOutTime = Input.CheckOutTime,
            Status = Input.Status,
            Notes = Input.Notes,

            InstitutionSupervisorID = user.Id
        };

        var exists = await _context.AttendanceRecords.AnyAsync(x =>
            x.PlacementID == placement.PlacementID &&
            x.AttendanceDate.Date == Input.AttendanceDate.Date);

        if (exists)
        {
            ModelState.AddModelError("", "تم تسجيل حضور لهذا اليوم مسبقاً");
            return Page();
        }

        _context.AttendanceRecords.Add(attendance);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تسجيل الحضور بنجاح";

        return RedirectToPage(
            "/Institution/CurrentTraining/StudentDetails",
            new { placementId = Input.PlacementID });
    }
}