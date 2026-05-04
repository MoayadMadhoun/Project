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
        [ForeignKey(nameof(RequestID))]
        public int RequestID { get; set; }
        public TrainingOpportunityRequest Request { get; set; }
        //SpecialtyID FK
        [Required(ErrorMessage = "Specialty is required")]
        [ForeignKey(nameof(SpecialtyID))]
        public int SpecialtyID { get; set; }
        public Specialty Specialty { get; set; }
    }

}
