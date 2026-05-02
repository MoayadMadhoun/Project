using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Specialty
    {
        [Key]
        public int SpecialtyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<OpportunitySpecialty> OpportunitySpecialties {get; set;}
        public ICollection<RequestSpecialty> RequestSpecialties { get; set; }
    }
}
