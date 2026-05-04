using System.ComponentModel.DataAnnotations;

namespace   Project.Models
{
    public class Skill
    {
        [Key]
        public int SkillID {  get; set; }
        [Required(ErrorMessage = "Skill name is required")]
        [MaxLength(200, ErrorMessage = "Skill name can't be more than 200 characters")]
        [MinLenght(2, ErrorMessage = "Skill name can't be less than 2 characters")]
        public string Name {  get; set; }
        [MaxLength(100, ErrorMessage = "Category can't be more than 200 characters")]
        public string? Category { get; set; }
        [MaxLength(500, ErrorMessage = "Description can't be more than 200 characters")]
        public string? Description { get; set; }
        public bool IsActive {  get; set; }
        public ICollection<StudentSkill> StudentSkills { get; set; }
        public ICollection<OpportunitySkill> OpportunitySkills { get; set; }
        public ICollection<RequestSkill> RequestSkills { get; set; }

    }
}
