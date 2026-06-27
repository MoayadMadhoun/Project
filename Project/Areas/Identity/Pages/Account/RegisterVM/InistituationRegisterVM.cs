// =============================================
// InistituationRegisterVM.cs
// =============================================
using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Identity.Pages.Account.RegisterVM
{
    public class InistituationRegisterVM : BaseRegisterVM
    {
        [Required(ErrorMessage = "العنوان حقل مطلوب")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "يجب أن يكون العنوان بين 8 و50 حرفاً")]
        public string Address { get; set; }

        public int? InstitutionID { get; set; }

        [Required(ErrorMessage = "نوع المؤسسة حقل مطلوب")]
        [StringLength(50)]
        public string InstituationType { get; set; }
    }
}
