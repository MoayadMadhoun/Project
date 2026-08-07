namespace Project.Pages.Institution.ViewModels
{
    public class StudentTrainingDetailsVM
    {
        public int PlacementID { get; set; }

        public int StudentID { get; set; }

        public string StudentName { get; set; } = "";

        public string StudentNumber { get; set; } = "";

        public string Specialty { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public decimal? GPA { get; set; }

        public string OpportunityTitle { get; set; } = "";

        public string InstitutionName { get; set; } = "";

        public string UniversitySupervisor { get; set; } = "";

        public string InstitutionSupervisor { get; set; } = "";

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ReportsCount { get; set; }

        public int EvaluationsCount { get; set; }

        public int AttendanceCount { get; set; }

        public int AbsentCount { get; set; }
    }
}
