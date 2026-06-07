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
        
        public int OpportunityID { get; set; }
        [ForeignKey(nameof(OpportunityID))]
        public TrainingOpportunity TrainingOpportunity { get; set; } = null!;   
        //SpecialtyID FK    
        [Required(ErrorMessage = "Specialty is required")]
        
        public int SpecialtyID { get; set; }
        [ForeignKey(nameof(SpecialtyID))]
        public Specialty Specialty { get; set; } = null!;
    }
}

