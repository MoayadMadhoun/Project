using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;
using Project.Services;

namespace Project.Pages.Institution.Reports;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IUploadFils _uploadDocxFile;
    private readonly IWebHostEnvironment _env;

    public CreateModel(
        ApplicationDbContext context,
        [FromKeyedServices("file")] IUploadFils uploadDocxFile,
        IWebHostEnvironment env)
    {
        _context = context;
        this._uploadDocxFile = uploadDocxFile;
        _env = env;
    }

    [BindProperty]
    public CreateReportVM Input { get; set; } = new();

    [BindProperty]
    public IFormFile? ReportFile { get; set; }

    public int PlacementID { get; set; }

    public int StudentID { get; set; }

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

        Input.PlacementID = placement.PlacementID;
        Input.StudentID = placement.StudentID;

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

        string? filePath = null;

        if (ReportFile != null)
        {
            filePath = _uploadDocxFile.UploadFile(
                ReportFile,
                "StudentReports"
            );
        }

        var report = new StudentReport
        {
            PlacementID = placement.PlacementID,
            StudentID = placement.StudentID,
            Type = Input.Type,
            Title = Input.Title,
            Content = Input.Content,
            FilePath = filePath,
            Status = StudentReport.StudentReportStatus.Submitted
        };

        _context.StudentReports.Add(report);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "تم رفع التقرير بنجاح";

        return RedirectToPage(
            "/Institution/CurrentTraining/StudentDetails",
            new
            {
                placementId = Input.PlacementID
            });
    }
}