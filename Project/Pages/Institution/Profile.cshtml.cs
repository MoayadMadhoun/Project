using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Models.Enums;
using Project.Pages.Institution.ViewModels;
using Project.Repository;

namespace Project.Pages.Institution
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly TrainingInstitutionRepository _institutionRepo;
        private readonly ApplicationDbContext _dbContext;

        public ProfileModel(UserManager<AspNetUser> userManager, TrainingInstitutionRepository institutionRepo, ApplicationDbContext dbContext) 
        {
            _userManager = userManager;
            _institutionRepo = institutionRepo;
            _dbContext = dbContext;
        }
        [BindProperty]
        public InstitutionProfileVM InstitutionProfile { get; set; } = new InstitutionProfileVM();
        public TrainingInstitution? CurrentInstitution { get;  set; }
        public SelectList InstitutionTypeList { get; set; }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                if (User == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {

                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                //int InstituationID = scope.InstitutionID;

                if (scope.InstitutionID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                int institutionId = scope.InstitutionID.Value;

                CurrentInstitution = await _institutionRepo.GetByIdAsync(institutionId);
                if (CurrentInstitution == null)
                {
                    return RedirectToPage("/Index");
                }
                var selectList = Enum.GetValues(typeof(InstitutionType))
                    .Cast<InstitutionType>()
                    .Select(s => new { Value = s.ToString(), Text = s.ToString() });

                InstitutionTypeList = new SelectList(selectList, "Value", "Text");
                InstitutionProfile = new InstitutionProfileVM {
                    Name = CurrentInstitution.Name,
                    Email = CurrentInstitution.Email,
                    PhoneNumber = CurrentInstitution.PhoneNumber,
                    Address = CurrentInstitution.Address,
                    InstitutionType = CurrentInstitution.InstitutionType,
                    ContactPersonName = CurrentInstitution.ContactPersonName,
                };
                return Page();
            }
            catch(Exception ex)
            {
                return RedirectToPage("/Index");
            }
        }
        public async Task<IActionResult> OnPost()
       {
            if (!ModelState.IsValid) {
                
                var selectList = Enum.GetValues(typeof(InstitutionType))
                    .Cast<InstitutionType>()
                    .Select(s => new { Value = s.ToString(), Text = s.ToString() });

                InstitutionTypeList = new SelectList(selectList, "Value", "Text");
               
                return Page(); 
            }
            if (User == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

            if (scope == null)
            {

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            

            if (scope.InstitutionID == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            int institutionId = scope.InstitutionID.Value;

            CurrentInstitution = await _institutionRepo.GetByIdAsync(institutionId);

            if (CurrentInstitution != null)
            {
                CurrentInstitution.Name = InstitutionProfile.Name;
                CurrentInstitution.ContactPersonName = InstitutionProfile.ContactPersonName;
                CurrentInstitution.Address = InstitutionProfile.Address;
                CurrentInstitution.PhoneNumber = InstitutionProfile.PhoneNumber;
                CurrentInstitution.Email = InstitutionProfile.Email;
                CurrentInstitution.InstitutionType = InstitutionProfile.InstitutionType;
                await _institutionRepo.UpdateAsync(CurrentInstitution);
            }
            return RedirectToPage();
        }
    }
}

