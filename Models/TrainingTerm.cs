using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class TrainingTerm
    {
        [Key]
        public int TermID { get; set; }
        public string Name { get; set; }
       
        public string AcademicYear { get; set; }
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive {  get; set; }
        public ICollection<TrainingOpportunity> Opportunities { get; set; }


    }
}
