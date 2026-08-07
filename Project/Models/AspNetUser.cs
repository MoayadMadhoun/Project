using Microsoft.AspNetCore.Identity;
using Project.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class AspNetUser : IdentityUser
    {
        [Required(ErrorMessage="Full name is required")]
        [MaxLength(200, ErrorMessage="Full name can't be more than 200 characters")]
        [MinLength(2, ErrorMessage = "Full name can't be less than 2 characters")]
        public string FullName {  get; set; }= string.Empty;
        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public Student? Student { get; set; }
        public AspNetRoleScope? RoleScope { get; set; }

        public University? University { get; set; }

        public TrainingInstitution? TrainingInstitution { get; set; }

        public string? ProfileImagePath { get; set; }
        public AccountType AccountType { get; set; }

    }
}
