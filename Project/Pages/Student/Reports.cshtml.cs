using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;
namespace Project.Pages.Student
{
    public class ReportsModel : PageModel
    {
        private readonly StudentsRepository studentsRepository;

        public ReportsModel(StudentsRepository StudentsRepository)
        {
            studentsRepository = StudentsRepository;
        }
        public PaginatedList<StudentReport> StudentReports { get; set; } 
        [BindProperty(SupportsGet =true)]
        public string? search { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? ReportType  { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? Reportstatus  { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

       

        public int? TotalQuary => StudentReports?.TotalCount ?? 0;
        public int? CurrentQuary => StudentReports?.Count() ?? 0;
       
        public async Task<IActionResult> OnGet()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId is null) { return RedirectToPage("/Identity/Account/Login"); }

            var student =await  studentsRepository.GetStudentByUserId(UserId);

            if (student is null) return Forbid();

            var stReports =  studentsRepository.GetStudentReportsAsync(student.StudentID);

            if (!string.IsNullOrWhiteSpace(search)) 
            {
                stReports = stReports.Where(sr=>EF.Functions.Like(sr.Title, $"%{search}%") );
            }

            if (ReportType.HasValue) {

                stReports = stReports.Where(sr =>(int) sr.Type == ReportType.Value);

            }
            if (Reportstatus.HasValue) {

                stReports = stReports.Where(sr => (int)sr.Status == Reportstatus.Value);
            }
            StudentReports =await PaginatedList<StudentReport>.CreateAsync(stReports, PageSize, PageIndex);

            return Page();
        }
         public async Task<IActionResult> OnGetOpenFile(int reportId) 
         {

            var report = await  studentsRepository.GetStudentReport(reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return NotFound();
            var FilePath = report?.FilePath ?? "" ;
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(FilePath,out string? contentType)) {
                contentType = "application/octet-stream"; 
            }
            return PhysicalFile(FilePath, contentType);

         }
        public async Task<IActionResult> OnPostDelete(int reportId) {

           await studentsRepository.DeleteStudentReportByReportId(reportId);
            return RedirectToPage();
        }
    
    }
}
