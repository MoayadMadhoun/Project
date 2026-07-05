using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels
{
    public class CreateReportVM
    {
        public int PlacementID { get; set; }

        public int StudentID { get; set; }

        public StudentReport.ReportType Type { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }
    }
}
