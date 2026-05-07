using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingApplication
    {
        [Key]
        public int ApplicationID { get; set; }
        //OpportunityID Fk
        [Required(ErrorMessage= "Training opportunity is required")]
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }= new TrainingOpportunity();
        //StudentID FK
        [Required(ErrorMessage = "Student is required")]
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }=new Student();

        public DateTime AppliedAt { get; set; } = DateTime.Now;
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? StudentNotes { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted ;
        public enum ApplicationStatus
        {
            Submitted, DepartmentApproved, DepartmentRejected, UniversityApproved, UniversityRejected,
            InstitutionRejected, Placed, Withdrawn
        }
        [Column(TypeName = "nvarchar(50)")]
        public Decision DepartmentDecision { get; set; } = Decision.Pending ;
        //
        [ForeignKey(nameof(DepartmentHeadID))]
        public string? DepartmentHeadID { get; set; }
        public AspNetUser? DepartmentHead { get; set; }
        //
  
        public DateTime? DepartmentReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? DepartmentReviewNotes { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public Decision UniversityAdminDecision { get; set; } = Decision.Pending ;
        //
        [ForeignKey(nameof(UniversityAdminID))]
        public string? UniversityAdminID { get; set; }
        public AspNetUser? UniversityAdmin { get; set; }
        //
        public DateTime? UniversityAdminReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? UniversityAdminNotes { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public Decision InstitutionDecision { get; set; } =  Decision.Pending ;
        //
        [ForeignKey(nameof(InstitutionOfficerID))]
        public string? InstitutionOfficerID { get; set; }
        public AspNetUser? InstitutionOfficer { get; set; }
        //
        public DateTime? InstitutionReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? InstitutionNotes { get; set; }


        public enum Decision
        {
            Pending, Approved, Rejected
        }
    }
}
