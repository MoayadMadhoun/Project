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
        public string UserID { get; set; }
        public AspNetUser User { get; set; }
        //RoleID FK
        [ForeignKey(nameof(RoleID))]
        public string RoleID {  get; set; }
        public IdentityRole Role { get; set; }
        //UniID FK
        [ForeignKey(nameof(UniversityID))]
        public int UniversityID { get; set; }
        public University University { get; set; }
        //DeptID FK
        [ForeignKey(nameof(DepartmentID))]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        //InstitID FK
        [ForeignKey(nameof(InstitutionID))]
        public int InstitutionID { get; set; } 
        public TrainingInstitution TrainingInstitution {  get; set; }
        public bool IsActive {  get; set; }
    }
}
