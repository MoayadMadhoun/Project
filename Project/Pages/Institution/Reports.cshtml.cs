using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;

namespace Project.Pages.Institution
{
    public class ReportsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly TrainingInstitutionRepository _institutionRepo;

        public ReportsModel(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, TrainingInstitutionRepository institutionRepo)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _institutionRepo = institutionRepo;
        }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SelectedReportType { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SelectedReportStatus { get; set; }
        public SelectList ReportStatusList { get; set; }
        public PaginatedList<StudentReport> Reports { get; set; } = new PaginatedList<StudentReport>(new List<StudentReport>(), 1, 0, 10);
        public TrainingInstitution? CurrentInstitution { get; private set; }
        [BindProperty(SupportsGet = true)]
        public int? StudentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PlacementId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                if (User == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {

                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                //int InstituationID = scope.InstitutionID;

                if (scope.InstitutionID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                int institutionId = scope.InstitutionID.Value;

                CurrentInstitution = await _institutionRepo.GetByIdAsync(institutionId);
                if (CurrentInstitution == null)
                {
                    return RedirectToPage("/Index");
                }
                var selectList = Enum.GetValues(typeof(StudentReport.StudentReportStatus))
                    .Cast<StudentReport.StudentReportStatus>()
                    .Select(s => new { Value = s.ToString(), Text = s.ToString() });

                ReportStatusList = new SelectList(selectList, "Value", "Text");
                var query = _dbContext.StudentReports

                 .Include(r => r.Student)

                 .Include(r => r.Placement)
                     .ThenInclude(p => p.TrainingOpportunity)

                 .Include(r => r.Placement)
                     .ThenInclude(p => p.TrainingInstitution)

                 .Include(r => r.UniversitySupervisor)

                 .Where(r => r.Placement.InstitutionID == institutionId)

                 .AsQueryable();

                if (StudentId.HasValue)
                {
                    query = query.Where(r =>
                        r.StudentID == StudentId.Value);
                }

                if (PlacementId.HasValue)
                {
                    query = query.Where(r =>
                        r.PlacementID == PlacementId.Value);
                }
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.Student.Name.Contains(SearchTerm));
                }
                if (SelectedReportStatus.HasValue)
                {
                    query = query.Where(r =>
                        (int)r.Status == SelectedReportStatus.Value);
                }
                if (SelectedReportType > 0)
                {
                    var type = SelectedReportType switch
                    {
                        1 => StudentReport.ReportType.Weekly,
                        2 => StudentReport.ReportType.Final,
                        _ => StudentReport.ReportType.Weekly
                    };
                    query = query.Where(r => r.Type == type);
                }
                query = SortOrder switch
                {
                    "Name" => query.OrderBy(a => a.Student.Name),
                    "Name_desc" => query.OrderByDescending(a => a.Student.Name),
                    _ => query.OrderBy(p => p.Student.StudentNumber),
                };
                Reports = await PaginatedList<StudentReport>.CreateAsync(query, PageSize, PageIndex);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");

            }
        }

        public async Task<IActionResult> OnGetOpenFile(int reportId)
        {
            var report = await _dbContext.StudentReports
                .FirstOrDefaultAsync(x => x.ReportID == reportId);

            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(report.FilePath,
                out string? contentType))
            {
                contentType = "application/octet-stream";
            }

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                report.FilePath.TrimStart('/'));

            return PhysicalFile(physicalPath, contentType);
        }
    }
}
