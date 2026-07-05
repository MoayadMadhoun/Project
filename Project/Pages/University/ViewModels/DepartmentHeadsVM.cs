using System.ComponentModel.DataAnnotations;

namespace Project.ViewModel
{
    public class DepartmentHeadsVM
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "اسم رئيس القسم مطلوب")]
        [StringLength(200,
                MinimumLength = 2,
                ErrorMessage = "اسم التخصص يجب أن يكون بين 2 و 200 حرف")]
        public string Name { get; set; }


        [Required (ErrorMessage ="الايميل مطلوب")]
        [EmailAddress(ErrorMessage ="This is Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="تحديد القسم الذي سيشرف عليه مهم ")]
        public int DepartmentId { get; set; }


        [Required(ErrorMessage = "رقم الهاتف مطلوب ")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "الرقم يجب ان يكون من عشر خانات كحد ادنى ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "يجب ان يحتوي على ارقام فقط )")]
        public string? PhoneNumber { get; set; }

        public bool isActive { get; set; }

    }
}
