using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class OpportunitySpecialty
    {
        [Key]
        public int OpportunitySpecialtyID { get; set; }
        
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }
        [ForeignKey(nameof(SpecialtyID))]
        public int SpecialtyID { get; set; }
        public Specialty Specialty { get; set; }
    }
}

