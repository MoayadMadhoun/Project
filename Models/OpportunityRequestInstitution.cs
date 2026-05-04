using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class OpportunityRequestInstitution
    {
        [Key]
        public int RequestInstitutionID { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [RegularExpression("^(Invited|Viewed|Reponded|Declined)$", ErrorMessage = "Status must be Invited, Viewed, Reponded or Declined")]
        public string Status { get; set; } = "Invited";

        public DateTime SentAt { get; set; }=DateTime.Now;
        public DateTime? RespondedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }
        //RequestID FK
        [Required(ErrorMessage = "Request is required")]
        [ForeignKey(nameof(RequestID))]
        public int RequestID { get; set; }
        public TrainingOpportunityRequest Request { get; set; }

        [Required(ErrorMessage = "Institution is required")]
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; }
        public TrainingInstitution TrainingInstitution { get; set; }


    }
}
