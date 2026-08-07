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
    public class DetailsModel : PageModel
    {
        private readonly DepartmentRepository _departmentRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;

        public DetailsModel(DepartmentRepository departmentRepository, ApplicationDbContext dbContext, UserManager<AspNetUser> userManager)
        {
            _departmentRepository = departmentRepository;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public Department? Department { get; set; }

        public AspNetUser? DepartmentHead { get; set; }
        public async Task OnGet([FromRoute] int departmentId)
        {
            Department = await _departmentRepository.GetByIdAsync(departmentId);
            var scope = await _dbContext.AspNetRoleScopes.Include(s=>s.User).Where(s => s.DepartmentID == departmentId).FirstOrDefaultAsync();
            DepartmentHead = scope?.User;
            
        }
    }
}
