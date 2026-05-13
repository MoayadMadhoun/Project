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
        
        public int OpportunityID { get; set; }
        [ForeignKey(nameof(OpportunityID))]
        public TrainingOpportunity TrainingOpportunity { get; set; }= new TrainingOpportunity();
        //StudentID FK
        [Required(ErrorMessage = "Student is required")]
        
        public int StudentID { get; set; }
        [ForeignKey(nameof(StudentID))]
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
        
        public string? DepartmentHeadID { get; set; }
        [ForeignKey(nameof(DepartmentHeadID))]
        public AspNetUser? DepartmentHead { get; set; }
        //
  
        public DateTime? DepartmentReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? DepartmentReviewNotes { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public Decision UniversityAdminDecision { get; set; } = Decision.Pending ;
        //
        
        public string? UniversityAdminID { get; set; }
        [ForeignKey(nameof(UniversityAdminID))]
        public AspNetUser? UniversityAdmin { get; set; }
        //
        public DateTime? UniversityAdminReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? UniversityAdminNotes { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public Decision InstitutionDecision { get; set; } =  Decision.Pending ;
        //
       
        public string? InstitutionOfficerID { get; set; }
        [ForeignKey(nameof(InstitutionOfficerID))]
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
