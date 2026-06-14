using Project.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels
{
    public class InstitutionProfileVM
    {
        [Required(ErrorMessage = "اسم المؤسسة مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم المؤسسة لا يمكن أن يتجاوز 100 حرف")]
        [MinLength(2, ErrorMessage = "اسم المؤسسة يجب أن يحتوي على حرفين على الأقل")]
        public string Name { get; set; } = string.Empty;
        public string? ContactPersonName
        {
            get; set;
        }
        [EmailAddress(ErrorMessage = "الايميل المدخل غير صالح")]
        public string? Email { get; set; }
        
        [StringLength(10, MinimumLength = 8, ErrorMessage = "الرقم يجب ان يكون من عشر خانات كحد ادنى ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "يجب ان يحتوي على ارقام فقط )")]
        public string? PhoneNumber { get; set; } 
        [MaxLength(500, ErrorMessage = "العنوان لا يمكن أن يتجاوز 500 حرف")]
        public string? Address { get; set; }
        public InstitutionType? InstitutionType { get; set; }


    }
}
