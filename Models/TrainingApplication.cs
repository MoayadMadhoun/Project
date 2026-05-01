using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingApplication
    {
        [Key]
        public int ApplicationID { get; set; }
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }
        public DateTime AppliedAt { get; set; }
        public string StudentNotes { get; set; }
        public string Status { get; set; }

        public string DepartmentDecision { get; set; }
        [ForeignKey(nameof(DepartmentHeadID))]
        public string? DepartmentHeadID { get; set; }
        public AspNetUser? DepartmentHead { get; set; }

  
        public DateTime DepartmentReviewedAt { get; set; }
        public string DepartmentReviewNotes { get; set; }

        public string UniversityAdminDecision { get; set; }
        [ForeignKey(nameof(UniversityAdminID))]
        public string? UniversityAdminID { get; set; }
        public AspNetUser? UniversityAdmin { get; set; }
        public DateTime UniversityAdminReviewedAt { get; set; }
        public string UniversityAdminNotes { get; set; }

        public string InstitutionDecision { get; set; }
        [ForeignKey(nameof(InstitutionOfficerID))]
        public string? InstitutionOfficerID { get; set; }
        public AspNetUser? InstitutionOfficer { get; set; }
        
        public DateTime InstitutionReviewedAt { get; set; }
        public string InstitutionNotes { get; set; }
    }
}
