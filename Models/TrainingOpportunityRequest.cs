using Project.Pages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingOpportunityRequest
    {

        [Key]
        public string RequestID { get; set; }
        [Required(ErrorMessage="Title is requied")]
        [MaxLength(300, ErrorMessage = "Title can't be more than 200 characters")]
        [MinLength(3, ErrorMessage = "Title can't be less than 3 characters")]
        public string Title { get; set; }
        [MaxLength(1000, ErrorMessage = "Description can't be more than 1000 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Requested seats number is requied")]
        
        public int RequestedSeats { get; set; }

        public DateTime? PreferredStartDate { get; set; }
        public DateTime? PreferredEndDate { get; set; }

        public DateTime? ApplicationDeadline { get; set; }
        
        [RegularExpression("^(Draft|Published|Closed|Cancelled)$", ErrorMessage = "Status must be Draft, Published, Closed, or Cancelled")]
        public string Status { get; set; } = "Draft";
        public DateTime CreatedAt { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }
        //DeptID Fk
        [ForeignKey(nameof(DepartmentID))]
        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }
        //UniID FK
        [Required(ErrorMessage="University is required")]
        [ForeignKey(nameof(UniversityID))]
        public int UniversityID { get; set; }
        public University University { get; set; }
        //TermID Fk
        [Required(ErrorMessage = "Training term is required")]
        [ForeignKey(nameof(TermID))]
        public int TermID { get; set; }
        public TrainingTerm TrainingTerm { get; set; }
        //Only one of those can be set one should be null
        [ForeignKey(nameof(UniversityAdminID))]
        public string UniversityAdminID { get; set; }
        public AspNetUser UniversityAdmin{ get; set;}
        [ForeignKey(nameof(DepartmentHeadID))]
        public string DepartmentHeadID { get; set; }
        public AspNetUser DepartmentHead{ get; set;}
       
        public ICollection<OpportunityRequestInstitution> OpportunityRequests { get; set; }
        public ICollection<RequestSpecialty> Specialties { get; set; }
        public ICollection<RequestSkill> Skills { get; set; }
        public ICollection<TrainingOpportunity> Opportunities { get; set; }
    }
}