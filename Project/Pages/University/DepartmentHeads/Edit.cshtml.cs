using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories;
using Project.Repostory;
using Project.ViewModel;

namespace Project.Pages.University.DepartmentHeads
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _repUniversity;
        private readonly DepartmentRepository _repDeparrtment;

        public EditModel(ApplicationDbContext dbContext,UniversityRepository repUniversity,DepartmentRepository repDeparrtment)
        {
            _dbContext = dbContext;
            _repUniversity = repUniversity;
            _repDeparrtment = repDeparrtment;
        }

        public AspNetUser? DepartmentHeadInfo { get; set; }

        [BindProperty]
        public DepartmentHeadsVM DepartmentHeadsVM { get; set; }

        public List<SelectListItem> DepartmentList { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            DepartmentHeadInfo = await _repUniversity.GetDpartmentHeadByUserId(id);
            var DepartmentHead = await _repUniversity.GetDpartmentHeadByUserId(id);
            DepartmentList = await _repDeparrtment.GetUniversityDepartmentsSelectListAsync(DepartmentHeadInfo.RoleScope.UniversityID.Value);
            if (DepartmentHead is null)
            {
                ModelState.AddModelError(nameof(DepartmentHead), "·« ÌÊÃœ —∆Ì” ﬁ”„ ");
                return NotFound();
            }
            FullViewModel(DepartmentHead, DepartmentHeadsVM);
            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if (!ModelState.IsValid)
            {
                DepartmentHeadInfo = await _repUniversity.GetDpartmentHeadByUserId(id);
                DepartmentList = await _repDeparrtment.GetUniversityDepartmentsSelectListAsync(DepartmentHeadInfo.RoleScope.UniversityID.Value);
                return Page();
            }
                

            var DepartmentHead = await _repUniversity.GetDpartmentHeadByUserId(id);
            if (DepartmentHead is null)
            {
                ModelState.AddModelError(nameof(DepartmentHead), "·« ÌÊÃœ —∆Ì” ﬁ”„ ");
                return NotFound();
            }

           await FullDepartmentHead(DepartmentHead,DepartmentHeadsVM);

            return Page();

        }

        private void FullViewModel(AspNetUser user,DepartmentHeadsVM vM)
        {
            vM.Name = user.FullName;
            vM.Email = user.Email;
            vM.isActive = user.IsActive;
            vM.PhoneNumber = user.PhoneNumber;
            
        }

        private async Task FullDepartmentHead(AspNetUser departmentHead, DepartmentHeadsVM DepartmentHeadsVM)
        {
            departmentHead.FullName = DepartmentHeadsVM.Name;
            departmentHead.Email = DepartmentHeadsVM.Email;
            departmentHead.PhoneNumber = DepartmentHeadsVM.PhoneNumber;
            departmentHead.IsActive = DepartmentHeadsVM.isActive;

            await _dbContext.SaveChangesAsync();
        }
    }
}
