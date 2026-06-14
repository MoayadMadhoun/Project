using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;

namespace Project.Pages.University.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public PaginatedList<StudentReport> Reports { get; set; }

        public SelectList ReportStatusList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedReportType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedReportStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? StudentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PlacementId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }
        public Models.Student? SelectedStudent { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });

            var scope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == user.Id &&
                    x.IsActive);

            if (scope == null)
                return Forbid();

            var query = _context.StudentReports

     .Include(r => r.Student)
         .ThenInclude(s => s.Department)

     .Include(r => r.Placement)
         .ThenInclude(p => p.TrainingOpportunity)

     .Include(r => r.Placement)
         .ThenInclude(p => p.TrainingInstitution)

     .Include(r => r.UniversitySupervisor)

     .Where(r =>
         r.Student.UniversityID ==
         scope.UniversityID.Value)

     .AsQueryable();

            if (StudentId.HasValue)
            {
                query = query.Where(r =>
                    r.StudentID ==
                    StudentId.Value);
            }
            if (StudentId.HasValue)
            {
                SelectedStudent = await _context.Students
                    .FirstOrDefaultAsync(s =>
                        s.StudentID == StudentId.Value);
            }
            // Department Head
            if (scope.DepartmentID.HasValue)
            {
                query = query.Where(r =>
                    r.Student.DepartmentID ==
                    scope.DepartmentID.Value);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(r =>
                    r.Student.Name.Contains(SearchTerm) ||
                    r.Title.Contains(SearchTerm));
            }

            // Type
            if (SelectedReportType.HasValue)
            {
                query = query.Where(r =>
                    (int)r.Type ==
                    SelectedReportType.Value);
            }

            // Status
            if (SelectedReportStatus.HasValue)
            {
                query = query.Where(r =>
                    (int)r.Status ==
                    SelectedReportStatus.Value);
            }

            // Student
            if (StudentId.HasValue)
            {
                query = query.Where(r =>
                    r.StudentID ==
                    StudentId.Value);
            }

            // Placement
            if (PlacementId.HasValue)
            {
                query = query.Where(r =>
                    r.PlacementID ==
                    PlacementId.Value);
            }

            query = SortOrder switch
            {
                "Name" => query.OrderBy(x =>
                    x.Student.Name),

                "Name_desc" => query.OrderByDescending(x =>
                    x.Student.Name),

                _ => query.OrderByDescending(x =>
                    x.SubmittedAt)
            };

            Reports =
                await PaginatedList<StudentReport>
                .CreateAsync(
                    query,
                    PageSize,
                    PageIndex);

            ReportStatusList =
                new SelectList(
                    Enum.GetValues(
                        typeof(
                            StudentReport.StudentReportStatus))
                    .Cast<StudentReport.StudentReportStatus>()
                    .Select(x => new
                    {
                        Value = (int)x,
                        Text = x.ToString()
                    }),
                    "Value",
                    "Text");

            return Page();
        }

        public async Task<IActionResult> OnGetOpenFile(int reportId)
        {
            var report = await _context.StudentReports
                .FirstOrDefaultAsync(r =>
                    r.ReportID == reportId);

            if (report == null ||
                string.IsNullOrEmpty(report.FilePath))
            {
                return NotFound();
            }

            var provider =
                new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(
                report.FilePath,
                out string? contentType))
            {
                contentType =
                    "application/octet-stream";
            }

            var physicalPath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    report.FilePath.TrimStart('/'));

            return PhysicalFile(
                physicalPath,
                contentType);
        }
    }
}