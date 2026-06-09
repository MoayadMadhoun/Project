using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using Project.Repository;

namespace Project.Pages.Institution
{
    public class CurrentTrainingModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StudentsRepository _studentsRepository;
        private readonly TrainingInstitutionRepository _institutionRepository;
        private readonly UserManager<AspNetUser> _userManager;

        public CurrentTrainingModel(ApplicationDbContext dbContext, StudentsRepository studentsRepository, TrainingInstitutionRepository institutionRepository, UserManager<AspNetUser> userManager)
        {
            _dbContext = dbContext;
            _studentsRepository = studentsRepository;
            _institutionRepository = institutionRepository;
            _userManager = userManager;
        }
        [BindProperty(SupportsGet =true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public PaginatedList<TrainingOpportunity> CurrentTrainings { get; set; } = new PaginatedList<TrainingOpportunity>(new List<TrainingOpportunity>(), 1, 0, 10);
        public TrainingInstitution? CurrentInstitution { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                AspNetRoleScope? scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

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
                var query = _institutionRepository.GetCurrentTrainingsForInstitution(institutionId);
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(tp => tp.Title.Contains(SearchTerm));
                }
                
                CurrentTrainings = await PaginatedList<TrainingOpportunity>.CreateAsync(query, PageSize, PageIndex);
                return Page();
                
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }

        }
    }
}
