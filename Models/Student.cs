using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        public string StudentNumber {  get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Level { get; set; }
        public decimal GPA { get; set; }
        public string Bio { get; set; }
        public string CVPath { get; set; }
        public string ProfileImagePath { get; set; }
        public string Status { get; set; }
        //
        [ForeignKey(nameof(DepartmentID))]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        //
        [ForeignKey(nameof(SpecialtyID))]
        public int SpecialtyID { get; set; }
        public Specialty? Specialty { get; set; }

        [ForeignKey(nameof(UserID))]
        public string UserID { get; set; }
        public AspNetUser User { get; set; }

        public ICollection<StudentSkill> Skills { get; set; }
        public ICollection<TrainingApplication> Applications { get; set; }
        
        public ICollection<PortfolioItem> PortfolioItems { get; set; }
        public ICollection<StudentReport> Reports { get; set; }




    }
}
