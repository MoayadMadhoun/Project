using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class FieldVisit
    {
        [Key]
        public int VisitID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string UniversitySupervisorID { get; set; }
        public AspNetUser UniversitySupervisor { get; set; }
        
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public string Notes { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
