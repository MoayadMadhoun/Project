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
        
        public int RequestID { get; set; }
        [ForeignKey(nameof(RequestID))]
        public TrainingOpportunityRequest Request { get; set; }= new TrainingOpportunityRequest();
        //SkillID FK
        [Required(ErrorMessage = "Skill is required")]
        
        public int SkillID { get; set; }
        [ForeignKey(nameof(SkillID))]
        public Skill Skill { get; set; }=new Skill();
        public bool IsRequired { get; set; } = true;
    }
}
