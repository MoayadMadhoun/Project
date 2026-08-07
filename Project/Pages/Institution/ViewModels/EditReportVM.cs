using Microsoft.AspNetCore.Http;
using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.Reports
{
    public class EditReportVM
    {
        public int ReportID { get; set; }

        public StudentReport.ReportType Type { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string? Content { get; set; }

        public IFormFile? ReportFile { get; set; }

        public string? ExistingFile { get; set; }
    }
}