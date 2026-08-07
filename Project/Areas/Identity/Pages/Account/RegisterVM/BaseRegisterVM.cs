// =============================================
// BaseRegisterVM.cs
// =============================================
using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Identity.Pages.Account.RegisterVM
{
    public class BaseRegisterVM
    {
        [Required(ErrorMessage = "الاسم الكامل حقل مطلوب")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "يجب أن يكون الاسم بين 8 و50 حرفاً")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف حقل مطلوب")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "يجب أن يكون رقم الهاتف بين 8 و10 أرقام")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "رقم الهاتف يجب أن يحتوي على أرقام فقط")]
        public string PhoneNumber { get; set; }
    }
}






