using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;
using Project.Repostory;

namespace Project.Pages.Institution
{
    public class SupervisorsModel : PageModel
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly TrainingInstitutionRepository _institutionRepository;
        private readonly UserManager<AspNetUser> _userManager;
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = string.Empty;
        public TrainingInstitution CurrentInstitution { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public bool IsActive { get; set; }
        public PaginatedList<AspNetRoleScope> Supervisors { get; set; }
        public string DepartmentHeadId { get; set; }
        public List<TrainingPlacement> TrainingPlacements { get; set; }

        public SupervisorsModel(ApplicationDbContext dbContext, TrainingInstitutionRepository institutionRepository, UserManager<AspNetUser> userManager)
        {

            _dbContext = dbContext;
            _institutionRepository = institutionRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {

            try
            {

                var user = await _userManager.GetUserAsync(User);
                AspNetRoleScope scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                if (scope.InstitutionID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                int institutionId = (int)scope.InstitutionID;

                CurrentInstitution = await _institutionRepository.GetByIdAsync(institutionId);
                if (CurrentInstitution == null)
                {
                    return RedirectToPage("/Index");
                }
                var query = _institutionRepository.GetSupervisorsForInstitution(institutionId);
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(a => a.User.FullName.Contains(SearchTerm));
                }
                if (IsActive == true)
                {
                    query = query.Where(a => a.IsActive);
                }
                else if (IsActive == false)
                {
                    query = query.Where(a => !a.IsActive);
                }
                TrainingPlacements =await _dbContext.TrainingPlacements.Include(tp=>tp.InstitutionSupervisor).ToListAsync();
                query = SortOrder switch
                {
                    "Name" => query.OrderBy(a => a.User.FullName),
                    "Name_desc" => query.OrderByDescending(a => a.User.FullName),
                    _ => query.OrderBy(p => p.UserID),

                };
                Supervisors = await PaginatedList<AspNetRoleScope>.CreateAsync(query, PageIndex, PageSize);
                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }

        }
    }
}
