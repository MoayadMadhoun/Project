using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentSkill
    {
        [Key]
        public string StudentSkillID {  get; set; }
        
        [RegularExpression("^(Beginner|Intermediate|Advanced)$", ErrorMessage = "Status must be Beginner, Intermediate, or Advanced")]
        public string? Level {  get; set; }
        //StudentID FK
        [Required(ErrorMessage="Student is required")]
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
        [ForeignKey(nameof(SkillID))]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }

    }
}
