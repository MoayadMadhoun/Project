using System.ComponentModel.DataAnnotations;

namespace Project.Areas.Identity.Pages.Account.RegisterVM
{
    public class InistituationRegisterVM:BaseRegisterVM
    {

        [Required(ErrorMessage = "Address is required field")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "The Address must be  at lest 8 letters long ")]
        public string Address { get; set; }

        public int? InstitutionID { get; set; }

    }
}
