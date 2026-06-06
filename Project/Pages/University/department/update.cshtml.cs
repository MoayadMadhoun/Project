using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;

namespace Project.Pages.University.department
{
    [Authorize(Roles = "UniversityTrainingAdmin")]
    public class updateModel : PageModel
    {
        private readonly DepartmentRepository _departmentRepository;

        public updateModel(DepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        [BindProperty]
        public string DepartmentName { get; set; }
        [BindProperty]
        public bool IsActive { get; set; }
        public Department Department { get; set; }
        public async Task OnGet([FromRoute] int departmentId)
        {
            Department = await _departmentRepository.GetByIdAsync(departmentId);
            if (Department != null)
            {
                DepartmentName = Department.Name;
                IsActive = Department.IsActive;
            }
           
        }
        public async Task OnPost([FromRoute] int departmentId)
        {
            
            Department.Name = DepartmentName;
            Department.IsActive = IsActive;
            if (Department != null)
            {
                await _departmentRepository.AddAsync(Department);
            }


        }
    }
}
