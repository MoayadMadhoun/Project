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
        
        public string UserID { get; set; } = string.Empty;
        [ForeignKey(nameof(UserID))]
        public AspNetUser User { get; set; } = new AspNetUser();
        //RoleID FK
       
        public string RoleID { get; set; } = string.Empty;
        [ForeignKey(nameof(RoleID))]
        public IdentityRole Role { get; set; } = new IdentityRole();
        //UniID FK
        
        public int UniversityID { get; set; }
        [ForeignKey(nameof(UniversityID))]
        public University University { get; set; } = new University();
        //DeptID FK
        
        public int DepartmentID { get; set; }
        [ForeignKey(nameof(DepartmentID))]
        public Department Department { get; set; } = new Department();
        //InstitID FK
        
        public int InstitutionID { get; set; }
        [ForeignKey(nameof(InstitutionID))]
        public TrainingInstitution TrainingInstitution { get; set; } = new TrainingInstitution();
        public bool IsActive {  get; set; }
    }
}
