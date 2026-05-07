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
        public TrainingPlacement TrainingPlacement { get; set; }= new TrainingPlacement();
        [Required(ErrorMessage = "University supervisor is required")]
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string UniversitySupervisorID { get; set; }=string.Empty;
        public AspNetUser UniversitySupervisor { get; set; }=new AspNetUser();
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
