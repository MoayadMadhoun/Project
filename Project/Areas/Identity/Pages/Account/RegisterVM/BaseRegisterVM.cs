using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Identity.Pages.Account.RegisterVM
{
    public class BaseRegisterVM
    {


        [Required(ErrorMessage ="Name is required field")]
        [StringLength(50,MinimumLength =8,ErrorMessage ="The Name must be  at lest 8 letters long ")]
        public string FullName { get; set; }


        


        [Required(ErrorMessage ="Phone Number is required field")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "Phone Nmber must be  at lest 10 letters long ")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone Nmber must be number (Only)")]
        public string PhoneNumber { get; set; }




        //[Required]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "رقم الهاتف يجب أن يكون 10 أرقام")]
        //public string PhoneNumber { get; set; }

    }
}
