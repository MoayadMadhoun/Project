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
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }= new Student();
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
        [ForeignKey(nameof(SkillID))]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }=new Skill();

    }
}
