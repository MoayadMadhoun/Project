using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        [Required(ErrorMessage="Student number is required")]
        [MaxLength(50, ErrorMessage = "Student number can't be more than 200 characters")]
        public string StudentNumber {  get; set; }
        [MaxLength(20, ErrorMessage = "Level can't be more than 200 characters")]
        public string? Level { get; set; }
        
        public decimal? GPA { get; set; }
        [MaxLength(1000, ErrorMessage = "Bio can't be more than 200 characters")]
        public string? Bio { get; set; }
        [MaxLength(500, ErrorMessage = "CV path can't be more than 200 characters")]
        public string? CVPath { get; set; }
        public string? ProfileImagePath { get; set; }
        [Required(ErrorMessage="Status is required")]
        [RegularExpression("^(Active|Inactive|Graduated)$",ErrorMessage="Status must be Active, Inactive, or Graduated")]
        public string Status { get; set; }
        //DeptID FK
        [Required(ErrorMessage = "Department is required")]
        [ForeignKey(nameof(DepartmentID))]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        //SpecialtyID Fk
        [ForeignKey(nameof(SpecialtyID))]
        public int? SpecialtyID { get; set; }
        public Specialty? Specialty { get; set; }
        //UserID FK
        [Required(ErrorMessage="User account is required")]
        [ForeignKey(nameof(UserID))]
        public string UserID { get; set; }
        public AspNetUser User { get; set; }

        public ICollection<StudentSkill> Skills { get; set; }
        public ICollection<TrainingApplication> Applications { get; set; }
        
        public ICollection<PortfolioItem> PortfolioItems { get; set; }
        public ICollection<StudentReport> Reports { get; set; }




    }
}
