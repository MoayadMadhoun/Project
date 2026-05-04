using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class AspNetUser : IdentityUser
    {
        [Required(ErrorMessage="Full name is required")]
        [MaxLength(200, ErrorMessage="Full name can't be more than 200 characters")]
        [MinLenght(2, ErrorMessage = "Full name can't be less than 2 characters")]
        public string FullName {  get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public Student? Student { get; set; }
        public AspNetRoleScope? RoleScope { get; set; }

    }
}
