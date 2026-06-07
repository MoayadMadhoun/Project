using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class RequestSpecialty
    {
        [Key]
        public int RequestSpecialtyID { get; set; }
        //RequestID FK
        [Required(ErrorMessage = "Request is required")]
        
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public TrainingOpportunityRequest Request { get; set; }= null!;      
        //SpecialtyID FK
        [Required(ErrorMessage = "Specialty is required")]
        
        public int SpecialtyID { get; set; }
        [ForeignKey(nameof(SpecialtyID))]
        public Specialty Specialty { get; set; } = null!;
    }

}
