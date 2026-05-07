using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class AspNetRoleScope
    {
        [Key]
        public int UserRoleScopeID { get; set; }
        //UserID FK
        [ForeignKey(nameof(UserID))]
        public string UserID { get; set; } = string.Empty;
        public AspNetUser User { get; set; } = new AspNetUser();
        //RoleID FK
        [ForeignKey(nameof(RoleID))]
        public string RoleID { get; set; } = string.Empty;
        public IdentityRole Role { get; set; } = new IdentityRole();
        //UniID FK
        [ForeignKey(nameof(UniversityID))]
        public int UniversityID { get; set; }
        public University University { get; set; } = new University();
        //DeptID FK
        [ForeignKey(nameof(DepartmentID))]
        public int DepartmentID { get; set; }
        public Department Department { get; set; } = new Department();
        //InstitID FK
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; } 
        public TrainingInstitution TrainingInstitution { get; set; } = new TrainingInstitution();
        public bool IsActive {  get; set; }
    }
}
