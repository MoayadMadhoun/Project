using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Identity.Pages.Account.RegisterVM
{
    public class StudentRegisterVM : BaseRegisterVM
    {
        [Required]
        public string StudentNumber { get; set; }
        [Required]
        public int SpecialtyId { get; set; }
        [Required]
        public int DepartmentID { get; set; }
        [Required]
        public int UniversityID { get; set; }

    }
}
