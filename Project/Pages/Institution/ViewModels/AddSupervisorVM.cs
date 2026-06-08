using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels
{
    public class AddSupervisorVM
    {
        [Required]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Display(Name = "الحساب فعال")]
        public bool IsActive { get; set; } = true;
    }
}
