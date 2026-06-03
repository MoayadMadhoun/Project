using Project.Models;

namespace Project.ViewModel
{
    public class TrainingApplicationVM
    {
        public int ApplicationID { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string StudentNumber { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string InstitutionName { get; set; } = string.Empty;

        public string OpportunityTitle { get; set; } = string.Empty;

        public DateTime AppliedAt { get; set; }

        public TrainingApplication.ApplicationStatus Status { get; set; }

        public TrainingApplication.Decision DepartmentDecision { get; set; }

        public TrainingApplication.Decision UniversityAdminDecision { get; set; }
    }
}
