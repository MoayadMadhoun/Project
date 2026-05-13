using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentSkill
    {
        [Key]
        public int StudentSkillID {  get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public StudentSkillLevel Level {  get; set; }
        public enum StudentSkillLevel
        {
            Beginner, Intermediate, Advanced
        }
        //StudentID FK
        [Required(ErrorMessage="Student is required")]
        
        public int StudentID { get; set; }
        [ForeignKey(nameof(StudentID))]
        public Student Student { get; set; }= new Student();
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
       
        public int SkillID { get; set; }
        [ForeignKey(nameof(SkillID))]
        public Skill Skill { get; set; }=new Skill();

    }
}
