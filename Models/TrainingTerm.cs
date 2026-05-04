using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class TrainingTerm
    {
        [Key]
        public int TermID { get; set; }
        [Required(ErrorMessage="Term name is required")]
        [MaxLength(100, ErrorMessage = "Term name can't be more than 200 characters")]
        [MinLength(3, ErrorMessage = "Term name can't be less than 3 characters")]
        public string Name { get; set; }
        [MaxLength(20, ErrorMessage = "Academic yeat can't be more than 20 characters")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Academic year must be in format YYYY-YYYY e.g. 2023-2024")]
        public string AcademicYear { get; set; }
        [Required(ErrorMessage="Start date is required")]
        public DateTime StartDate {  get; set; }
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
        public bool IsActive {  get; set; }
        public ICollection<TrainingOpportunity> Opportunities { get; set; }


    }
}
