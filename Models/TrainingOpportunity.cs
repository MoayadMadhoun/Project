using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingOpportunity
    {
        [Key]
        public int OpportunityID { get; set; }
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; }
        public TrainingInstitution TrainingInstitution { get; set; }
        [ForeignKey(nameof(TermID))]
        public int TermID { get; set; }
        public TrainingTerm TrainingTerm { get; set; }

        [ForeignKey(nameof(RequestID))]
        public int? RequestID { get; set; }
        public TrainingOpportunityRequest? Request { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        [ForeignKey(nameof(InstitutionOfficerID))]
        public string InstitutionOfficerID { get; set; } 
        public AspNetUser InstitutionOfficer { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
