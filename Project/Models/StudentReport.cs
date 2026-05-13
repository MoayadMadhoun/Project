using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentReport
    {
        [Key]
        public int ReportID { get; set; }

        [Required(ErrorMessage = "Placement is required")]
        
        public int PlacementID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public TrainingPlacement Placement { get; set; } = new TrainingPlacement(); 

        [Required(ErrorMessage = "Student is required")]
        
        public int StudentID { get; set; }
        [ForeignKey(nameof(StudentID))]
        public Student Student { get; set; }= new Student();

        [Column(TypeName = "nvarchar(50)")]
        public ReportType Type { get; set; }
        public enum ReportType
        {
            Weekly, Final
        }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot be more than 300 characters")]
        [MinLength(3, ErrorMessage = "Title cannot be less than 3 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000, ErrorMessage = "Content cannot be more than 5000 characters")]
        public string? Content { get; set; }

        
        public string? FilePath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(50)")]
        public StudentReportStatus Status { get; set; } = StudentReportStatus.Submitted;
        public enum StudentReportStatus
        {
            Submitted, Reviewed, Approved, Rejected
        }

        public string? UniversitySupervisorID { get; set; }
        
        public AspNetUser? UniversitySupervisor { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]

        [MaxLength(1000, ErrorMessage = "Review notes cannot be more than 1000 characters")]
        public string? ReviewNotes { get; set; }
    }
}
