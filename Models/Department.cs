using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        [Required(ErrorMessage = "Department name is required")]
        [MaxLength(200, ErrorMessage = "Department name can't be more than 200 characters")]
        [MinLenght(2, ErrorMessage = "Department name can't be less than 2 characters")]
        public string Name { get; set; }  
        public bool IsActive { get; set; }
        [Required(ErrorMessage="University is required")]
        [ForeignKey(nameof(UniversityID))]
        public int UniversityID { get; set; }
        public University University { get; set; }

        public ICollection<Student> Students { get; set; }

    }
}
