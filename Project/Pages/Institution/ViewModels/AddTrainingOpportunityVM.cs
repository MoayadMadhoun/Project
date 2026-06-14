using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels
{
    public class AddTrainingOpportunityVM : IValidatableObject
    {
        [Required(ErrorMessage = "عنوان الفرصة مطلوب")]
        [MaxLength(200, ErrorMessage = "عنوان الفرصة لا يمكن أن يتجاوز 200 حرف")]
        [MinLength(2, ErrorMessage = "عنوان الفرصة يجب أن يحتوي على حرفين على الأقل")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "وصف الفرصة مطلوب")]
        [MaxLength(500, ErrorMessage = "وصف الفرصة لا يمكن أن يتجاوز 500 حرف")]
        [MinLength(10, ErrorMessage = "وصف الفرصة يجب أن يحتوي على 10 أحرف على الأقل")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "عدد المقاعد مطلوب")]
        [Range(1, 1000, ErrorMessage = "عدد المقاعد يجب أن يكون أكبر من صفر")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "تاريخ بداية التدريب مطلوب")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ نهاية التدريب مطلوب")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "مكان التدريب مطلوب")]
        [MaxLength(300, ErrorMessage = "مكان التدريب لا يمكن أن يتجاوز 300 حرف")]
        [MinLength(2, ErrorMessage = "مكان التدريب يجب أن يحتوي على حرفين على الأقل")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "فترة التدريب مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار فترة التدريب")]
        public int TermId { get; set; }

        public int? RequestId { get; set; }
        public List<int> SelectedSpecialties { get; set; } = new();

        public List<int> SelectedSkills { get; set; } = new();

        public Dictionary<int, bool> SkillTypes { get; set; } = new();


        [Required(ErrorMessage = "حالة الفرصة مطلوبة")]
        public Opportunity Status { get; set; } = Opportunity.Open;

        public enum Opportunity
        {
            [Display(Name = "مفتوحة")]
            Open = 1,

            [Display(Name = "مغلقة")]
            Closed = 2,

            [Display(Name = "ملغاة")]
            Cancelled = 3
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "تاريخ نهاية التدريب يجب أن يكون بعد تاريخ البداية",
                    new[] { nameof(EndDate) });
            }

            if (StartDate.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "لا يمكن اختيار تاريخ بداية في الماضي",
                    new[] { nameof(StartDate) });
            }
        }
    }
}