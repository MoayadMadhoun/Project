using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        [Required(ErrorMessage = "Student Name is required")]
        [MaxLength(50, ErrorMessage = "Student Name can't be more than 200 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage="Student number is required")]
        [MaxLength(50, ErrorMessage = "Student number can't be more than 50 characters")]
        public string StudentNumber {  get; set; }
        [MaxLength(20, ErrorMessage = "Level can't be more than 20 characters")]
        public string? Level { get; set; }
        
        public decimal? GPA { get; set; }
        [MaxLength(1000, ErrorMessage = "Bio can't be more than 1000 characters")]
        public string? Bio { get; set; }
        [MaxLength(500, ErrorMessage = "CV path can't be more than 500 characters")]
        public string? CVPath { get; set; }
        public string? ProfileImagePath { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public StudentStatus Status { get; set; } = StudentStatus.Active;

        public enum StudentStatus
        {
            Active, Inactive,  Graduated
        }
        //DeptID FK
        [Required(ErrorMessage = "Department is required")]
        [ForeignKey(nameof(DepartmentID))]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }= new Department();   
        //SpecialtyID Fk
        [ForeignKey(nameof(SpecialtyID))]
        public int? SpecialtyID { get; set; }
        public Specialty? Specialty { get; set; }
        //UserID FK
        [Required(ErrorMessage = "User account is required")]
        [ForeignKey(nameof(UserID))]
        public string UserID { get; set; } = string.Empty;
        public AspNetUser User { get; set; }=new AspNetUser();  

        public ICollection<StudentSkill> Skills { get; set; } = new HashSet<StudentSkill>();                    
        public ICollection<TrainingApplication> Applications { get; set; } = new HashSet<TrainingApplication>();    
        
        public ICollection<PortfolioItem> PortfolioItems { get; set; } = new HashSet<PortfolioItem>();
        public ICollection<StudentReport> Reports { get; set; } = new HashSet<StudentReport>();




    }
}
