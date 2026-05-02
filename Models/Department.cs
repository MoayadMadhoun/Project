using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        public string Name { get; set; }  
        public bool IsActive { get; set; }

        [ForeignKey(nameof(UniversityID))]
        public int UniversityID { get; set; }
        public University University { get; set; }

        public ICollection<Student> Students { get; set; }

    }
}
