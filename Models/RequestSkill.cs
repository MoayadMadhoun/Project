using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class RequestSkill
    {
        [Key]
        public int RequestSkillID {  get; set; }
        [ForeignKey(nameof(RequestID))]
        public int RequestID { get; set; }
        public TrainingOpportunityRequest Request { get; set; }
        //
        [ForeignKey(nameof(SkillID))]
        public int SkillID { get; set; }
        public Skill Skill { get; set; }
        public bool IsRequired {  get; set; }
    }
}
