

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.ViewModels.StudentReports;

namespace Project.Pages.Student.Reports
{


    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public CreateStudentReportVM VM { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var placement = await _context.TrainingPlacements
                .Include(p => p.TrainingOpportunity)
                .Include(p => p.TrainingInstitution)
                .Include(p => p.UniversitySupervisor)
                .FirstOrDefaultAsync(p => p.StudentID == id);

            if (placement == null)
                return NotFound();

            VM = new CreateStudentReportVM
            {
                StudentId = id,

                OpportunityTitle =
                    placement.TrainingOpportunity.Title,

                InstitutionName =
                    placement.TrainingInstitution.Name,

                UniversitySupervisorName =
                    placement.UniversitySupervisor?.FullName ?? "-"
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await ReloadTrainingInfo();
                return Page();
            }

            var placement = await _context.TrainingPlacements
                .FirstOrDefaultAsync(p => p.StudentID == VM.StudentId);

            if (placement == null)
                return NotFound();

            string? filePath = null;

            if (VM.ReportFile != null)
            {
                var folder =
                    Path.Combine(_environment.WebRootPath,
                                 "uploads",
                                 "reports");

                Directory.CreateDirectory(folder);

                var fileName =
                    $"{Guid.NewGuid()}{Path.GetExtension(VM.ReportFile.FileName)}";

                var fullPath =
                    Path.Combine(folder, fileName);

                using var stream =
                    new FileStream(fullPath, FileMode.Create);

                await VM.ReportFile.CopyToAsync(stream);

                filePath = $"/uploads/reports/{fileName}";
            }

            var report = new StudentReport
            {
                StudentID = VM.StudentId,
                PlacementID = placement.PlacementID,

                Type = VM.Type,
                Title = VM.Title,
                Content = VM.Content,

                FilePath = filePath,

                SubmittedAt = DateTime.Now,

                Status = StudentReport.StudentReportStatus.Submitted,

                UniversitySupervisorID =
                    placement.UniversitySupervisorID
            };

            _context.StudentReports.Add(report);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task ReloadTrainingInfo()
        {
            var placement = await _context.TrainingPlacements
                .Include(p => p.TrainingOpportunity)
                .Include(p => p.TrainingInstitution)
                .Include(p => p.UniversitySupervisor)
                .FirstOrDefaultAsync(p => p.StudentID == VM.StudentId);

            if (placement == null)
                return;

            VM.OpportunityTitle =
                placement.TrainingOpportunity.Title;

            VM.InstitutionName =
                placement.TrainingInstitution.Name;

            VM.UniversitySupervisorName =
                placement.UniversitySupervisor?.FullName ?? "-";
        }
    }

}