using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentSkill
    {
        [Key]
        public string StudentSkillID {  get; set; }
        public string Level {  get; set; }
        //
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }
        //
        [ForeignKey(nameof(SkillID))]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }

    }
}
