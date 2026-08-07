using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("Department")]
        public int DepartmentID { get; set; }

        public Department Department { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<OpportunitySpecialty> OpportunitySpecialties { get; set; } = new List<OpportunitySpecialty>();
        public ICollection<RequestSpecialty> RequestSpecialties { get; set; } = new List<RequestSpecialty>();
    }
}
