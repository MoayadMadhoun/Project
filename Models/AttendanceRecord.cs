using Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceID { get; set; }
        //PlacmentID FK
        [Required(ErrorMessage = "Placement is required")]
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }
        [Required(ErrorMesssage= "Attendance date is required")]
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        [RegularExpression("^(Present|Absent|Late|Excused)$", ErrorMessage = "Status must be Present, Absent, Late, or Excused")]

        public string Status { get; set; } = "Present";
        [Required(ErrorMessage = "Institution supervisor is required")]

        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string InstitutionSupervisorID { get; set; }
        public AspNetUser InstitutionSupervisor { get; set; }
        [MaxLength(500, ErrorMessage = "Notes can't be more than 500 characters")]
        public string? Notes { get; set; }
    }
}
