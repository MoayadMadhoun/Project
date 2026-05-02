using Project.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceID { get; set; }
        [ForeignKey(nameof(PlacementID))]
        public int PlacementID { get; set; }
        public TrainingPlacement TrainingPlacement { get; set; }

        public DateTime AttendanceDate { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string Status { get; set; }
     
        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string InstitutionSupervisorID { get; set; }
        public AspNetUser InstitutionSupervisor { get; set; }
        public string Notes { get; set; }
    }
}
