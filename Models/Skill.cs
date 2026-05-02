using System.ComponentModel.DataAnnotations;

namespace   Project.Models
{
    public class Skill
    {
        [Key]
        public int SkillID {  get; set; }
        public string Name {  get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsActive {  get; set; }
        public ICollection<StudentSkill> StudentSkills { get; set; }
        public ICollection<OpportunitySkill> OpportunitySkills { get; set; }
        public ICollection<RequestSkill> RequestSkills { get; set; }

    }
}
