using Project.Pages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingOpportunityRequest
    {

        [Key]
        public int RequestID { get; set; }
        [Required(ErrorMessage = "Title is requied")]
        [MaxLength(300, ErrorMessage = "Title can't be more than 200 characters")]
        [MinLength(3, ErrorMessage = "Title can't be less than 3 characters")]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000, ErrorMessage = "Description can't be more than 1000 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Requested seats number is requied")]
        
        public int RequestedSeats { get; set; }

        public DateTime? PreferredStartDate { get; set; }
        public DateTime? PreferredEndDate { get; set; }

        public DateTime? ApplicationDeadline { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public RequestStatus Status { get; set; } = RequestStatus.Draft;
        public enum RequestStatus
        {
            Draft, Published, Closed, Cancelled
        }
        public DateTime CreatedAt { get; set; } = new DateTime();
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }
        //DeptID Fk
        
        public int? DepartmentID { get; set; }
        [ForeignKey(nameof(DepartmentID))]
        public Department? Department { get; set; }
        //UniID FK
        [Required(ErrorMessage="University is required")]
        
        public int UniversityID { get; set; }
        [ForeignKey(nameof(UniversityID))]
        public University University { get; set; }=null!;
        //TermID Fk
        [Required(ErrorMessage = "Training term is required")]
        
        public int TermID { get; set; }
        [ForeignKey(nameof(TermID))]
        public TrainingTerm TrainingTerm { get; set; } = null!;
        //Only one of those can be set one should be null
        
        public string? UniversityAdminID { get; set; } = string.Empty;
        [ForeignKey(nameof(UniversityAdminID))]
        public AspNetUser? UniversityAdmin { get; set; } = null!;
        public string? DepartmentHeadID { get; set; } = string.Empty;
        [ForeignKey(nameof(DepartmentHeadID))]
        public AspNetUser? DepartmentHead { get; set; } = null!;

        public ICollection<OpportunityRequestInstitution> OpportunityRequests { get; set; } = new List<OpportunityRequestInstitution>();
        public ICollection<RequestSpecialty> Specialties { get; set; } = new List<RequestSpecialty>();   
        public ICollection<RequestSkill> Skills { get; set; }= new List<RequestSkill>();
        public ICollection<TrainingOpportunity> Opportunities { get; set; } = new List<TrainingOpportunity>();
    }
}