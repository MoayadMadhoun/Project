using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class TrainingOpportunityRequest
    {

        [Key]
        public string RequestID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequestedSeats { get; set; }

        public DateTime PreferredStartDate { get; set; }
        public DateTime PreferredEndDate { get; set; }

        public DateTime ApplicationDeadline { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Notes { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }

        [ForeignKey(nameof(UniversityID))]
        public int UniversityID { get; set; }
        public University University { get; set; }

        [ForeignKey(nameof(TermID))]
        public int TermID { get; set; }
        public TrainingTerm TrainingTerm { get; set; }
        //Only one of those can be set one should be null
        [ForeignKey(nameof(UniversityAdminID))]
        public string UniversityAdminID { get; set; }
        public AspNetUser UniversityAdmin{ get; set;}
        [ForeignKey(nameof(DepartmentHeadID))]
        public string DepartmentHeadID { get; set; }
        public AspNetUser DepartmentHead{ get; set;}
       
        public ICollection<OpportunityRequestInstitution> OpportunityRequests { get; set; }
        public ICollection<RequestSpecialty> Specialties { get; set; }
        public ICollection<RequestSkill> Skills { get; set; }
        public ICollection<TrainingOpportunity> Opportunities { get; set; }
    }
}