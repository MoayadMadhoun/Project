using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class TrainingInstitution
    {
        [Key]
        public int InstituationID {  get; set; }
        [Required(ErrorMessage = "Instituation name is required")]
        [MaxLength(200, ErrorMessage = "Instituation name can't be more than 200 characters")]
        [MinLenght(2, ErrorMessage = "Instituation name can't be less than 2 characters")]
        public string Name { get;  set; }
        [MaxLength(100, ErrorMessage = "Type can't be more than 200 characters")]
        public string? Type { get; set; }
        [MaxLength(300, ErrorMessage = "Address can't be more than 200 characters")]
        public string? Address { get; set; }
        [MaxLength(50, ErrorMessage = "Phone number can't be more than 200 characters")]
        public string? PhoneNumber { get; set; }
        [MaxLength(150, ErrorMessage = "Email can't be more than 200 characters")]
        [EmailAddress(ErrorMessage="Invalid email format")]
        public string? Email { get; set; }
        [MaxLength(200, ErrorMessage = "Contant person name can't be more than 200 characters")]
        public string? ContactPersonName { get; set; }
        public bool IsActive {  get; set; }
        public ICollection<TrainingOpportunity> TrainingOpportunities { get; set; }



    }
}
