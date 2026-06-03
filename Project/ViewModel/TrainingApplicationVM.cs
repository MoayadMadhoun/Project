using Project.Models;

namespace Project.ViewModel
{
    public class TrainingApplicationVM
    {
        public int ApplicationID { get; set; }

        public string StudentName { get; set; }
        public string StudentNumber { get; set; }

        public string InstitutionName { get; set; }

        public DateTime AppliedAt { get; set; }

        public TrainingApplication.ApplicationStatus Status { get; set; }

        public TrainingApplication.Decision DepartmentDecision { get; set; }
    }
}
