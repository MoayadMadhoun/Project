using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentReport
    {
        [Key]
        public int ReportID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }

        public string ReportType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string? UniversitySupervisorID { get; set; }
        public AspNetUser? UniversitySupervisor { get; set; }
     
        public string ReviewNotes { get; set; }
    }
}
