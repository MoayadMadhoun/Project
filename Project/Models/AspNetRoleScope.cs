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
        
        public string UserID { get; set; } 
        [ForeignKey(nameof(UserID))]
        public AspNetUser User { get; set; } 
        //RoleID FK
       
        public string RoleID { get; set; } 
        [ForeignKey(nameof(RoleID))]
        public IdentityRole Role { get; set; } 
        //UniID FK
        
        public int? UniversityID { get; set; }
        [ForeignKey(nameof(UniversityID))]
        public University? University { get; set; } 
        //DeptID FK
        
        public int? DepartmentID { get; set; }
        [ForeignKey(nameof(DepartmentID))]
        public Department? Department { get; set; } 
        //InstitID FK
        
        public int? InstitutionID { get; set; }
        [ForeignKey(nameof(InstitutionID))]
        public TrainingInstitution? TrainingInstitution { get; set; } 

        public int? StudentID { get; set; }
        public bool IsActive {  get; set; }
    }
}
