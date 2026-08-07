using Project.Models;

namespace Project.Pages.Institution.ViewModels
{
    public class EditAttendanceVM
    {
        public int AttendanceID { get; set; }

        public int PlacementID { get; set; }

        public DateTime AttendanceDate { get; set; }

        public TimeSpan? CheckInTime { get; set; }

        public TimeSpan? CheckOutTime { get; set; }

        public AttendanceRecord.AttendanceStatus Status { get; set; }

        public string? Notes { get; set; }
    }
}