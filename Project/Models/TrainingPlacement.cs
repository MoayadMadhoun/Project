using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingPlacement
    {
        [Key]
        public int PlacementID { get; set; }
        //OpportunityID FK
        [Required(ErrorMessage= "Training opportunity is required") ]
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }= new TrainingOpportunity();
        //TrainingApplication FK
        [Required(ErrorMessage = "Training application is required")]
        [ForeignKey(nameof(ApplicationID))]
        public int ApplicationID { get; set; }
        public TrainingApplication TrainingApplication { get; set; } = new TrainingApplication();   
        //StudentID FK
        [Required(ErrorMessage = "Student is required")]
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; } = new Student();
        //InstitutionID FK
        [Required(ErrorMessage = "Institution opportunity is required")]
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; }
        public TrainingInstitution TrainingInstitution { get; set; } = new TrainingInstitution();
        //TermID FK
        [Required(ErrorMessage = "Training Term is required")]
        [ForeignKey(nameof(TermID))]
        public int TermID { get; set; }
        public TrainingTerm TrainingTerm { get; set; } = new TrainingTerm();
        //
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string? UniversitySupervisorID { get; set; }
        public AspNetUser? UniversitySupervisor { get; set; }
        //
        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string? InstitutionSupervisorID {get; set;}
        public AspNetUser? InstitutionSupervisor { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public PlacementStatus Status { get; set; } = PlacementStatus.Assigned;
        public enum PlacementStatus
        {
            Assigned, InProgress, Completed, Cancelled
        }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new HashSet<AttendanceRecord>();     
        public ICollection<FieldVisit> FieldVisits { get; set; } = new HashSet<FieldVisit>();
        public ICollection<StudentEvaluation> StudentEvaluations { get; set; } = new HashSet<StudentEvaluation>();
        public ICollection<StudentReport> StudentReports { get; set; } = new HashSet<StudentReport>();
    }
}
