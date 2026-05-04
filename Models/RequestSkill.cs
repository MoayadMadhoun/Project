using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class RequestSkill
    {
        [Key]
        public int RequestSkillID {  get; set; }
        //RequestID FK
        [Required(ErrorMessage = "Request is required")]
        [ForeignKey(nameof(RequestID))]
        public int RequestID { get; set; }
        public TrainingOpportunityRequest Request { get; set; }
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
        [ForeignKey(nameof(SkillID))]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}
