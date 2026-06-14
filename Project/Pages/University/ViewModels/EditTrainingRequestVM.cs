using System.ComponentModel.DataAnnotations;
using static Project.Models.TrainingOpportunityRequest;

namespace Project.Pages.University.ViewModels
{
    public class EditTrainingRequestVM
    {

        [Required(ErrorMessage = "عنوان الفرصة مطلوب")]
        [MaxLength(200, ErrorMessage = "عنوان الفرصة لا يمكن أن يتجاوز 200 حرف")]
        [MinLength(2, ErrorMessage = "عنوان الفرصة يجب أن يحتوي على حرفين على الأقل")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "وصف الفرصة لا يمكن أن يتجاوز 500 حرف")]
        [MinLength(10, ErrorMessage = "وصف الفرصة يجب أن يحتوي على 10 أحرف على الأقل")]
        public string? Description { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime? PreferredStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PreferredEndDate { get; set; }
        [Required(ErrorMessage = "فترة التدريب مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار فترة التدريب")]
        public int TermID { get; set; }
        [Required(ErrorMessage = " الحالة مطلوبة")]

        public RequestStatus Status { get; set; } = RequestStatus.Draft;
        [MaxLength(1000, ErrorMessage = " الملاحظات لا يمكن أن تتجاوز 1000 حرف")]
        public string? Notes { get; set; }
        [Required(ErrorMessage = "عدد المقاعد المطلوبة مطلوب")]
        [Range(0, 200, ErrorMessage = "عدد المقاعد لا يمكن ان يكون اقل من صفر")]
        public int RequestedSeats { get; set; }
        public DateTime? ApplicationDeadline { get; set; }

    }
}
