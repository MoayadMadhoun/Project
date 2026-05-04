using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentEvaluation
    {
        [Key]
        public int EvaluationID { get; set; }
        [Required(ErrorMessage = "Placement is required")]
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string? UniversitySupervisorID { get; set; }
        public AspNetUser? UniversitySupervisor { get; set; }
        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string? InstitutionSupervisorID { get; set; }
        public AspNetUser? InstitutionSupervisor { get; set; }

        [Required(ErrorMessage = "Evaluation type is required")]
        [RegularExpression("^(UniversityMid|UniversityFinal|InstitutionMid|InstitutionFinal)$",
        ErrorMessage = "Invalid evaluation type")]
        public string EvaluationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Evaluation date is required")]
        public DateTime EvaluationDate { get; set; }
        [Required(ErrorMessage = "Score is required")]
        public decimal Score { get; set; }
        [Required(ErrorMessage = "Max score is required")]
        public decimal MaxScore { get; set; }
        [MaxLength(2000, ErrorMessage = "Notes cannot be more than 2000 characters")]
        public string Notes { get; set; }
        public string EvaluationPdfPath { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Draft|Submitted|Approved|Rejected)$", ErrorMessage = "Invalid status value")]
        public string Status { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
