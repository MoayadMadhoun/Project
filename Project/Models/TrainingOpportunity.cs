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
        
        public int InstitutionID { get; set; }
        [ForeignKey(nameof(InstitutionID))]
        public TrainingInstitution TrainingInstitution { get; set; } = new TrainingInstitution();
        //TermID Fk
        [Required(ErrorMessage = "Training term is required")]
        
        public int TermID { get; set; }
        [ForeignKey(nameof(TermID))]
        public TrainingTerm TrainingTerm { get; set; } = new TrainingTerm();
        //RequestID FK
        public int? RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
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
        
        public string InstitutionOfficerID { get; set; } = string.Empty;
        [ForeignKey(nameof(InstitutionOfficerID))]
        public AspNetUser InstitutionOfficer { get; set; } = new AspNetUser();
        [Column(TypeName = "nvarchar(50)")]
        public Opportunity Status { get; set; } = Opportunity.Open;
        public enum Opportunity
        {
            Open =1, Closed=2, Cancelled=3
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<RequestSpecialty> Specialties { get; set; } = new List<RequestSpecialty>();
    }
}
