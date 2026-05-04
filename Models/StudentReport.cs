using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentReport
    {
        [Key]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "Placement is required")]
        [ForeignKey(nameof(PlacementId))]
        public int PlacementId { get; set; }
        public TrainingPlacement Placement { get; set; };

        [Required(ErrorMessage = "Student is required")]
        [ForeignKey(nameof(StudentId))]
        public int StudentId { get; set; }

        public Student Student { get; set; };


        [Required(ErrorMessage = "Report type is required")]
        [RegularExpression("^(Weekly|Final)$", ErrorMessage = "Report type must be Weekly or Final")]
        public string ReportType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot be more than 300 characters")]
        [MinLength(3, ErrorMessage = "Title cannot be less than 3 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000, ErrorMessage = "Content cannot be more than 5000 characters")]
        public string? Content { get; set; }

        
        public string? FilePath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

       
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Submitted|Reviewed|Approved|Rejected)$", ErrorMessage = "Invalid status value")]
        public string Status { get; set; } = "Submitted";

        public string? UniversitySupervisorId { get; set; }
        [ForeignKey(nameof(UniversitySupervisorId))]
        public ApplicationUser? UniversitySupervisor { get; set; }

        [MaxLength(1000, ErrorMessage = "Review notes cannot be more than 1000 characters")]
        public string? ReviewNotes { get; set; }
    }
}
