using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.University.ViewModels
{
        public class CreateSpecialtyVM
        {
            [Required(ErrorMessage = "اسم التخصص مطلوب")]
            [StringLength(200,
                MinimumLength = 2,
                ErrorMessage = "اسم التخصص يجب أن يكون بين 2 و 200 حرف")]
            [DisplayName("الاسم")]
        public string Name { get; set; } = string.Empty;

            [StringLength(500,
                ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف")]
        [DisplayName("الوصف")]
        public string? Description { get; set; }

            [StringLength(100,
                ErrorMessage = "التصنيف لا يمكن أن يتجاوز 100 حرف")]
        [DisplayName("التصنيف")]
        public string? Category { get; set; }

            [Required(ErrorMessage = "يجب اختيار القسم")]
            [Range(1, int.MaxValue,
                ErrorMessage = "يجب اختيار القسم")]
            public int DepartmentID { get; set; }

            public bool IsActive { get; set; } = true;
        }
}
