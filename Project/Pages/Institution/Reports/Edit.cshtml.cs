using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Services;

namespace Project.Pages.Institution.Reports
{
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
        public EditReportVM Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int reportId)
        {
            var report = await _context.StudentReports
                .FirstOrDefaultAsync(x => x.ReportID == reportId);

            if (report == null)
                return NotFound();

            Input = new EditReportVM
            {
                ReportID = report.ReportID,
                Type = report.Type,
                Title = report.Title,
                Content = report.Content,
                ExistingFile = report.FilePath
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var report = await _context.StudentReports
                .FirstOrDefaultAsync(x =>
                    x.ReportID == Input.ReportID);

            if (report == null)
                return NotFound();

            report.Type = Input.Type;
            report.Title = Input.Title;
            report.Content = Input.Content;

            if (Input.ReportFile != null)
            {
                report.FilePath =
                    _uploadFile.UploadFile(
                        Input.ReportFile,
                        "StudentReports");
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "تم تعديل التقرير بنجاح";

            return RedirectToPage(
                "/Institution/Reports");
        }
    }
}