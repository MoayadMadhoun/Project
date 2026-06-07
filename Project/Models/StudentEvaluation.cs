using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class StudentEvaluation
    {
        [Key]
        public int EvaluationID { get; set; }
        [Required(ErrorMessage = "Placement is required")]
       
        public int PlacementID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public TrainingPlacement TrainingPlacement { get; set; } = null!;
        
        public string? UniversitySupervisorID { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]
        public AspNetUser? UniversitySupervisor { get; set; }
        
        public string? InstitutionSupervisorID { get; set; }
        [ForeignKey(nameof(InstitutionSupervisorID))]
        public AspNetUser? InstitutionSupervisor { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public EvaluationType Type { get; set; } 
        public enum EvaluationType
        {
            UniversityMid, UniversityFinal, InstitutionMid, InstitutionFinal
        }

        [Required(ErrorMessage = "Evaluation date is required")]
        public DateTime EvaluationDate { get; set; }
        [Required(ErrorMessage = "Score is required")]
        public decimal Score { get; set; }
        [Required(ErrorMessage = "Max score is required")]
        public decimal MaxScore { get; set; }
        [MaxLength(2000, ErrorMessage = "Notes cannot be more than 2000 characters")]
        public string Notes { get; set; } = string.Empty;
        public string EvaluationPdfPath { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(50)")]
        public StudentEvaluationStatus Status { get; set; } = StudentEvaluationStatus.Draft;
        public enum StudentEvaluationStatus
        {
            Draft, Submitted, Approved, Rejected
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
