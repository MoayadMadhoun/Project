using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;
using Project.Repostory;

namespace Project.Pages.Institution
{
    public class IndexModel : PageModel
    {
        private readonly TrainingInstitutionRepository _institutionRepo;
        private readonly ApplicationDbContext _dbContext;
        private readonly UniversityRepository _universityRepo;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)]
        public TrainingInstitution CurrentInstitution { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int UniversityId { get; set; }
        public SelectList Universities { get; set; }
        private readonly UserManager<AspNetUser> _userManager;

        public PaginatedList<TrainingApplication> TrainingApplications { get; set; }
        public IndexModel(TrainingInstitutionRepository institutionRepo, ApplicationDbContext dbContext, UniversityRepository universityRepo, UserManager<AspNetUser> userManager)
        {
            _institutionRepo = institutionRepo;
            _dbContext = dbContext;
            _universityRepo = universityRepo;
            _userManager = userManager;
        }
        public async Task<ActionResult> OnGet()
        {
            try
            {
                if (User == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }

                var scope = await _dbContext.AspNetRoleScopes.FirstOrDefaultAsync(s => s.UserID == user.Id);
                        //.FirstOrDefaultAsync(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {
                  
                    return RedirectToPage("Identity/Account/Login");
                }
              //int InstituationID = scope.InstitutionID;

                if (scope.InstitutionID == null)
                {
                    return RedirectToPage("Identity/Account/Login");
                }

                int institutionId = scope.InstitutionID.Value;

                CurrentInstitution = await _institutionRepo.GetByIdAsync(institutionId);
                if (CurrentInstitution == null)
                {
                    return RedirectToPage("/Index");
                }
                var query = _institutionRepo.GetApplicationsForInstitution(institutionId);

                var unis = await _universityRepo.GetAllAsync();

                Universities = new SelectList(unis, "UniversityID", "Name");

                if (UniversityId > 0)
                {
                    query = query.Where(a => a.Student.Department.UniversityID == UniversityId);
                }
                if (SearchTerm != null)
                {
                    query = query.Where(a => a.TrainingOpportunity.Title.Contains(SearchTerm));
                }

                query = SortOrder switch
                {
                    "Date" => query.OrderBy(a => a.AppliedAt),
                    "Date_desc" => query.OrderByDescending(a => a.AppliedAt),
                    "Status" => query.OrderBy(a => a.Status),
                    "Status_desc" => query.OrderByDescending(a => a.Status),
                    _ => query.OrderBy(p => p.AppliedAt),

                };
                TrainingApplications = await PaginatedList<TrainingApplication>.CreateAsync(query, PageSize, PageIndex);
                return Page();

            }
            catch
            {
                return RedirectToPage("/Index");
            }
        }
    }
}