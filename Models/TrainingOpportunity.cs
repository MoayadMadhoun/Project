using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingOpportunity
    {
        [Key]
        public int OpportunityID { get; set; }
        //InstitutionID FK
        [Required(ErrorMessage = "Institution is required")]
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; }
        public TrainingInstitution TrainingInstitution { get; set; } = new TrainingInstitution();
        //TermID Fk
        [Required(ErrorMessage = "Training term is required")]
        [ForeignKey(nameof(TermID))]
        public int TermID { get; set; }
        public TrainingTerm TrainingTerm { get; set; } = new TrainingTerm();
        //RequestID FK
        [Required(ErrorMessage = "Request is required")]

        [ForeignKey(nameof(RequestID))]
        public int? RequestID { get; set; }
        public TrainingOpportunityRequest? Request { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title can't be more than 200 characters")]
        [MinLength(2, ErrorMessage = "Title can't be less than 2 characters")]
        public string Title { get; set; } = string.Empty;
        [MaxLength(500, ErrorMessage = "Description can't be more than 500 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Capicity is required")]
        public int Capacity { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
        [MaxLength(300, ErrorMessage = "Location can't be more than 300 characters")]
        public string? Location { get; set; }
        //InstitutionOfficerID FK
        [Required(ErrorMessage = "InstitutionOfficer is required")]
        [ForeignKey(nameof(InstitutionOfficerID))]
        public string InstitutionOfficerID { get; set; } = string.Empty;
        public AspNetUser InstitutionOfficer { get; set; } = new AspNetUser();
        [Column(TypeName = "nvarchar(50)")]
        public Opportunity Status { get; set; } = Opportunity.Open;
        public enum Opportunity
        {
            Open, Closed, Concelled
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
