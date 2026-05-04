using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class OpportunitySpecialty
    {
        [Key]
        public int OpportunitySpecialtyID { get; set; }
        //OpportunityID FK
        [Required(ErrorMessage = "Training opportunity is required")]
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }
        //SpecialtyID FK
        [Required(ErrorMessage = "Specialty is required")]
        [ForeignKey(nameof(SpecialtyID))]
        public int SpecialtyID { get; set; }
        public Specialty Specialty { get; set; }
    }
}

