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
        
        public int OpportunityID { get; set; }
        [ForeignKey(nameof(OpportunityID))]
        public TrainingOpportunity TrainingOpportunity { get; set; } = new TrainingOpportunity();
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
        
        public int SkillID { get; set; }
        [ForeignKey(nameof(SkillID))]
        public Skill Skill { get; set; }=new Skill();
        public bool IsRequired { get; set; } = true;
    }
}

