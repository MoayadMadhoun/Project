using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class University
    {
        [Key]
        public int UniversityID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set;  }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<TrainingOpportunityRequest> TrainingOpportunityRequests {  get; set; }
    }
}
