using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class FieldVisit
    {
        [Key]
        public int VisitID { get; set; }
        [Required(ErrorMessage = "Placement is required")]
        
        public int PlacementID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public TrainingPlacement TrainingPlacement { get; set; }= null!;
        [Required(ErrorMessage = "University supervisor is required")]
        
        public string UniversitySupervisorID { get; set; }=string.Empty;
        [ForeignKey(nameof(UniversitySupervisorID))]
        public AspNetUser UniversitySupervisor { get; set; }=null!;
        [Required(ErrorMessage = "Visit date is required")]
        public DateTime VisitDate { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public VisitType Type { get; set; } = VisitType.Initial;
        public enum VisitType
        {
            Initial, Final, FollowUp
        }
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
        [MaxLength(1000, ErrorMessage = "Notes can't be more than 1000 characters")]
        public string? Notes { get; set; }

        public string? AttachmentPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
