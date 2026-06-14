using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels
{
    public class CreateAttendanceVM
    {
        public int PlacementID { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        public TimeSpan? CheckInTime { get; set; }

        public TimeSpan? CheckOutTime { get; set; }

        public AttendanceRecord.AttendanceStatus Status { get; set; }
            = AttendanceRecord.AttendanceStatus.Present;

        public string? Notes { get; set; }
    }
}
