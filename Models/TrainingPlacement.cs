using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingPlacement
    {
        [Key]
        public int PlacementID { get; set; }
        [ForeignKey(nameof(OpportunityID))]
        public int OpportunityID { get; set; }
        public TrainingOpportunity TrainingOpportunity { get; set; }
        [ForeignKey(nameof(ApplicationID))]
        public int ApplicationID { get; set; }
        public TrainingApplication TrainingApplication { get; set; }
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; }
        public TrainingInstitution TrainingInstitution { get; set; }
        [ForeignKey(nameof(TermID))]
        public int TermID { get; set; }
        public TrainingTerm TrainingTerm { get; set; }
        [ForeignKey(nameof(UniversitySupervisorID))]
        public string UniversitySupervisorID { get; set; }
        public AspNetUser UniversitySupervisor { get; set; }
        [ForeignKey(nameof(InstitutionSupervisorID))]
        public string InstitutionSupervisorID {get; set;}
        public AspNetUser InstitutionSupervisor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        //FinalGrade,//computed from evaluation, not manual 
        public string Notes { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
        public ICollection<FieldVisit> FieldVisits { get; set; }
        public ICollection<StudentEvaluation> StudentEvaluations { get; set; }
        public ICollection<StudentReport> StudentReports { get; set; }
    }
}
