using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class FieldVisit
    {
        [Key]
        public int VisitID { get; set; }
        [Required(ErrorMessage = "Placement is required")]
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }
        [Required(ErrorMessage = "University supervisor is required")]
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string UniversitySupervisorID { get; set; }
        public AspNetUser UniversitySupervisor { get; set; }
        [Required(ErrorMessage = "Visit date is required")]
        public DateTime VisitDate { get; set; }
        [RegularExpression("^(Initial|Final|FollowUp)$", ErrorMessage = "Decision must be Initial, Final, or FollowUp")]
        [Required(ErrorMessage="Visit type is required")]
        public string VisitType { get; set; } = "Initial";
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }

        public string? AttachmentPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
