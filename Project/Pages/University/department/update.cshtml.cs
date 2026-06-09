using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.University.department
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class updateModel : PageModel
    {
        private readonly DepartmentRepository _departmentRepository;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public updateModel(DepartmentRepository departmentRepository, UserManager<AspNetUser> userManager, ApplicationDbContext dbContext)
        {
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [BindProperty]
        public string DepartmentName { get; set; }
        [BindProperty]
        public bool IsActive { get; set; }
        public Department? Department { get; set; }
        public async Task OnGet([FromRoute] int departmentId)
        {
            Department = await _departmentRepository.GetByIdAsync(departmentId);
            if (Department != null)
            {
                DepartmentName = Department.Name;
                IsActive = Department.IsActive;
            }
           
        }
        public async Task<IActionResult> OnPost([FromRoute] int departmentId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }
            try
            {


                AspNetUser user = new AspNetUser();

                user = await _userManager.GetUserAsync(User);
                AspNetRoleScope scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                if (scope.UniversityID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }


                int universityId = (int)scope.UniversityID;
               
                Department = await _departmentRepository.GetByIdAsync(departmentId);
                if (Department?.UniversityID!= universityId)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                    if (Department != null)
                {

                    Department.Name = DepartmentName;
                    Department.IsActive = IsActive;
                    await _departmentRepository.UpdateAsync(Department);
                }
                return RedirectToPage("/University/Departments");
            }
            catch (Exception ex) { return RedirectToPage("/Index"); }


            
        }
    }
}
