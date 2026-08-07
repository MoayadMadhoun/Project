// =============================================
// StudentRegisterVM.cs
// =============================================
using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Identity.Pages.Account.RegisterVM
{
    public class StudentRegisterVM : BaseRegisterVM
    {
        [Required(ErrorMessage = "الرقم الجامعي حقل مطلوب")]
        public string StudentNumber { get; set; }

        [Required(ErrorMessage = "يرجى اختيار التخصص")]
        public int SpecialtyId { get; set; }

        [Required(ErrorMessage = "يرجى اختيار القسم")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "يرجى اختيار الجامعة")]
        public int UniversityID { get; set; }
    }
}
