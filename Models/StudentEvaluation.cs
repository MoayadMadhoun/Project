using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentEvaluation
    {
        [Key]
        public int EvaluationID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string? UniversitySupervisorID { get; set; }
        public AspNetUser? UniversitySupervisor { get; set; }
        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string? InstitutionSupervisorID { get; set; }
        public AspNetUser? InstitutionSupervisor { get; set; }
       
        public string EvaluationType { get; set; }
        public DateTime EvaluationDate { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public string Notes { get; set; }
        public string EvaluationPdfPath { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
