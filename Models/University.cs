using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class University
    {
        [Key]
        public int UniversityID { get; set; }
        [Required(ErrorMessage = "University name is required")]
        [MaxLength(200, ErrorMessage = "University name can't be more than 200 characters")]
        [MinLenght(2, ErrorMessage = "University name can't be less than 2 characters")]
        public string Name { get; set; }
        [MaxLength(300, ErrorMessage = "Address can't be more than 300 characters")]
        public string? Address { get; set; }
        [MaxLength(50, ErrorMessage = "Phone number can't be more than 50 characters")]
       
        public string? PhoneNumber { get; set;  }
        [EmailAddress(ErrorMessage="Invalid email format")]
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<TrainingOpportunityRequest> TrainingOpportunityRequests {  get; set; }
    }
}
