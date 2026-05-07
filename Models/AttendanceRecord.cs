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
        [Required(ErrorMessage= "Attendance date is required")]
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        public enum AttendanceStatus
        {
            Present, Absent, Late, Excused
        }
        [Required(ErrorMessage = "Institution supervisor is required")]

        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string InstitutionSupervisorID { get; set; }
        public AspNetUser InstitutionSupervisor { get; set; }
        [MaxLength(500, ErrorMessage = "Notes can't be more than 500 characters")]
        public string? Notes { get; set; }
    }
}
