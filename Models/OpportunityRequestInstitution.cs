using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class OpportunityRequestInstitution
    {
        [Key]
        public int RequestInstitutionID { get; set; }
        public string Status {  get; set; }
        public DateTime SentAt { get; set; }
        public DateTime RespondedAt { get; set; }
        public string Notes { get; set; }

        [ForeignKey(nameof(RequestID))]
        public int RequestID { get; set; }
        public TrainingOpportunityRequest Request { get; set; }


        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; }
        public TrainingInstitution TrainingInstitution { get; set; }


    }
}
