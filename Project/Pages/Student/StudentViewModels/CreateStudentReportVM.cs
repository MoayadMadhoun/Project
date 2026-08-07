using Microsoft.AspNetCore.Http;
using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels.StudentReports
{
    public class CreateStudentReportVM
    {
        public int StudentId { get; set; }

        public string OpportunityTitle { get; set; } = string.Empty;

        public string InstitutionName { get; set; } = string.Empty;

        public string UniversitySupervisorName { get; set; } = string.Empty;

        [Required]
        public StudentReport.ReportType Type { get; set; }

        [Required]
        [MaxLength(300)]
        [MinLength(3)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string? Content { get; set; }

        public IFormFile? ReportFile { get; set; }
    }
}