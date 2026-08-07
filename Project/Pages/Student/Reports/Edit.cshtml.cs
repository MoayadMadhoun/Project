using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Student.StudentViewModels;
using Project.Services;
using Project.ViewModels.StudentReports;

namespace Project.Pages.Student.Reports;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IUploadFils _uploadFile;

    public EditModel(
        ApplicationDbContext context,
        [FromKeyedServices("file")] IUploadFils uploadFile)
    {
        _context = context;
        _uploadFile = uploadFile;
    }

    [BindProperty]
    public EditStudentReportVM VM { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int reportId)
    {
        var report = await _context.StudentReports
            .Include(r => r.Placement)
                .ThenInclude(p => p.TrainingOpportunity)
            .Include(r => r.Placement)
                .ThenInclude(p => p.TrainingInstitution)
            .Include(r => r.UniversitySupervisor)
            .FirstOrDefaultAsync(r => r.ReportID == reportId);

        if (report == null)
            return NotFound();

        if (report.Status != StudentReport.StudentReportStatus.Submitted)
            return Forbid();

        VM = new EditStudentReportVM
        {
            ReportID = report.ReportID,
            StudentId = report.StudentID,

            Type = report.Type,
            Title = report.Title,
            Content = report.Content,

            CurrentFilePath = report.FilePath,

            OpportunityTitle =
                report.Placement.TrainingOpportunity.Title,

            InstitutionName =
                report.Placement.TrainingInstitution.Name,

            UniversitySupervisorName =
                report.UniversitySupervisor?.FullName ?? "-"
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var report = await _context.StudentReports
            .FirstOrDefaultAsync(r => r.ReportID == VM.ReportID);

        if (report == null)
            return NotFound();

        if (report.Status != StudentReport.StudentReportStatus.Submitted &&
            report.Status != StudentReport.StudentReportStatus.Rejected)
        {
            return Forbid();
        }

        report.Title = VM.Title;
        report.Content = VM.Content;
        report.Type = VM.Type;

        if (VM.ReportFile != null)
        {
            report.FilePath =
                _uploadFile.UploadFile(
                    VM.ReportFile,
                    "StudentReports");
        }

        report.SubmittedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تعديل التقرير بنجاح";

        return RedirectToPage("/Student/Reports");
    }
}