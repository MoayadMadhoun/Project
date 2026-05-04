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
        public TrainingOpportunity TrainingOpportunity { get; set; }
        //StudentID FK
        [Required(ErrorMessage = "Student is required")]
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? StudentNotes { get; set; }
        [RegularExpression("^(Submitted|DepartmentApproved|DepartmentRejected|UniversityApproved|UniversityRejected|InstitutionRejected|Placed|WithDrawn)$",
            ErrorMessage = "Invalid status vlaue")]
        public string Status { get; set; } = "Submitted";
        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Decision must be Pending, Approved, or Rejected")]
        public string DepartmentDecision { get; set; } = "Pending";
        //
        [ForeignKey(nameof(DepartmentHeadID))]
        public string? DepartmentHeadID { get; set; }
        public AspNetUser? DepartmentHead { get; set; }
        //
  
        public DateTime? DepartmentReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? DepartmentReviewNotes { get; set; }
        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Decision must be Pending, Approved, or Rejected")]

        public string UniversityAdminDecision { get; set; } = "Pending";
        //
        [ForeignKey(nameof(UniversityAdminID))]
        public string? UniversityAdminID { get; set; }
        public AspNetUser? UniversityAdmin { get; set; }
        //
        public DateTime? UniversityAdminReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? UniversityAdminNotes { get; set; }
        [RegularExpression("^(Pending|Approved|Rejected)$", ErrorMessage = "Decision must be Pending, Approved, or Rejected")]

        public string InstitutionDecision { get; set; } = "Pending";
        //
        [ForeignKey(nameof(InstitutionOfficerID))]
        public string? InstitutionOfficerID { get; set; }
        public AspNetUser? InstitutionOfficer { get; set; }
        //
        public DateTime? InstitutionReviewedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? InstitutionNotes { get; set; }
    }
}
