using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
    public class AspNetUser : IdentityUser
    {
        public string FullName {  get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
