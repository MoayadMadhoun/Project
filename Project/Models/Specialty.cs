using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Specialty
    {
        [Key]
        public int SpecialtyID { get; set; }
        [Required(ErrorMessage = "Specialty name is required")]
        [MaxLength(200, ErrorMessage = "Specialty name can't be more than 200 characters")]
        [MinLength(2, ErrorMessage = "Specialty name can't be less than 2 characters")]
        public string Name { get; set; }=string.Empty;
        [MaxLength(500, ErrorMessage = "Description can't be more than 200 characters")]
        public string? Description { get; set; }
        [MaxLength(100, ErrorMessage = "Category can't be more than 200 characters")]
        public string? Category { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
        public ICollection<OpportunitySpecialty> OpportunitySpecialties { get; set; } = new HashSet<OpportunitySpecialty>();
        public ICollection<RequestSpecialty> RequestSpecialties { get; set; } = new HashSet<RequestSpecialty>();
    }
}
