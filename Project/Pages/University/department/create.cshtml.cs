using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Project.Data;
using Project.Models;
using Project.Repositories;
using Project.Repostory;

namespace Project.Pages.University.department
{
    [Authorize(Roles ="UniversityTrainingAdmin")]
    public class createModel : PageModel
    {
        private readonly DepartmentRepository _departmentRepository;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _universityRepository;

        public createModel(DepartmentRepository departmentRepository, UserManager<AspNetUser> userManager, ApplicationDbContext dbContext, UniversityRepository universityRepository)
        {
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _dbContext = dbContext;
            _universityRepository = universityRepository;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            public string Name { get; set; }
            public bool IsActive { get; set; }
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPost() 
        {
            if (ModelState.IsValid)
            {
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
                    Models.University uni = await _universityRepository.GetByIdAsync(universityId);
                    if (uni!=null)
                    {
                        var department = new Department();
                        department.Name = Input.Name;
                        department.IsActive = Input.IsActive;
                        department.UniversityID = universityId;
                        await _departmentRepository.AddAsync(department);
                        return RedirectToPage("/University/Departments");
                    } else return RedirectToPage("/Index");
                }
                catch
                {
                    return RedirectToPage("/Index");
                }
            }
            return RedirectToPage();

        }
    }
}
