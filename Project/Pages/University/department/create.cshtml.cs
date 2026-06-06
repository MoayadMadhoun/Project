using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.University.department
{
    [Authorize(Roles ="UniversityTrainingAdmin")]
    public class createModel : PageModel
    {
        private readonly DepartmentRepository _departmentRepository;

        public createModel(DepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        [BindProperty]
        public string DepartmentName { get; set; }
        [BindProperty]
        public bool IsActive { get; set; }
        public void OnGet()
        {

        }
        public async Task OnPost([FromRoute] int universityId) 
        {
            var newDepartment = new Department();
            newDepartment.Name = DepartmentName;
            newDepartment.IsActive = IsActive;
            newDepartment.UniversityID = universityId;
            await _departmentRepository.AddAsync(newDepartment);

           

        }
    }
}
