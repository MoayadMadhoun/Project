using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Student.StudentViewModels
{
    public class EditStudentReportVM
    {
        public int ReportID { get; set; }

        public int StudentId { get; set; }

        public StudentReport.ReportType Type { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        public IFormFile? ReportFile { get; set; }

        public string? CurrentFilePath { get; set; }

        public string OpportunityTitle { get; set; } = "";

        public string InstitutionName { get; set; } = "";

        public string UniversitySupervisorName { get; set; } = "";
    }
}
