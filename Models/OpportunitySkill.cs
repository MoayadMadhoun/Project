using Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class OpportunitySkill
    {
        [Key]
       public int OpportunitySkillID {  get; set; }
        //OpportunityID FK
        [Required(ErrorMessage = "Training opportunity is required")]
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
        [ForeignKey(nameof(SkillID))]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}

