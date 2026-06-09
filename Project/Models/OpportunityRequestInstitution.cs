using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class OpportunityRequestInstitution
    {
        [Key]
        public int RequestInstitutionID { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public OpportunityRequestInstitutionStatus Status { get; set; } = OpportunityRequestInstitutionStatus.Invited;
        public enum OpportunityRequestInstitutionStatus
        {
            Invited, Viewed, Reponded, Declined
        }


        public DateTime SentAt { get; set; }=DateTime.Now;
        public DateTime? RespondedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }
        //RequestID FK
        [Required(ErrorMessage = "Request is required")]
        
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public TrainingOpportunityRequest Request { get; set; } = null! ;         

        [Required(ErrorMessage = "Institution is required")]
        
        public int InstitutionID { get; set; }
        [ForeignKey(nameof(InstitutionID))]
        public TrainingInstitution TrainingInstitution { get; set; } = null!;


    }
}
