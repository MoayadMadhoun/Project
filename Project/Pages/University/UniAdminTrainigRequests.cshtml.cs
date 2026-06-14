using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Models.Enums;
using Project.Repositories;
using Project.ViewModel;

namespace Project.Pages.University
{
    public class UniAdminTrainigRequestsModel : PageModel
    {
        private readonly DepartmentRepository departmentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public UniAdminTrainigRequestsModel(
            DepartmentRepository  departmentRepository,
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            this.departmentRepository = departmentRepository;
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public TrainingApplication.ApplicationStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public TrainingApplication.Decision? DepartmentDecisionFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public PaginatedList<TrainingApplicationVM> Applications { get; set; }
            = new(new List<TrainingApplicationVM>(), 1, 0, 10);

        public SelectList StatusList { get; set; }

        public SelectList DecisionList { get; set; }

        public SelectList DepartmentList { get; set; }



        public async Task OnGetAsync()
        {
            StatusList =
                EnumExtensions.GetEnumSelectList<TrainingApplication.ApplicationStatus>();

            DecisionList =
                EnumExtensions.GetEnumSelectList<TrainingApplication.Decision>();

            DepartmentList = new SelectList(await departmentRepository.GetAllAsync(), "DepartmentID", "Name");

            var userId = _userManager.GetUserId(User);

            var roleScope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == userId &&
                    x.UniversityID != null &&
                    x.IsActive);

            if (roleScope == null)
            {
                Applications = new PaginatedList<TrainingApplicationVM>(
                    new List<TrainingApplicationVM>(),
                    1,
                    0,
                    PageSize);

                return;
            }

            var universityId = roleScope.UniversityID;

            var query = _context.TrainingApplications
                .Include(x => x.Student)
                    .ThenInclude(s => s.Department)
                .Include(x => x.TrainingOpportunity)
                    .ThenInclude(o => o.TrainingInstitution)
                .Where(x =>
                    x.Student.UniversityID == universityId &&
                    x.UniversityAdminDecision ==
                    TrainingApplication.Decision.Pending)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(x =>
                    x.Student.Name.Contains(Search) ||
                    x.Student.StudentNumber.Contains(Search) ||
                    x.Student.Department.Name.Contains(Search) ||
                    x.TrainingOpportunity.TrainingInstitution.Name.Contains(Search) ||
                    x.TrainingOpportunity.Title.Contains(Search));
            }

            // Status Filter
            if (StatusFilter.HasValue)
            {
                query = query.Where(x =>
                    x.Status == StatusFilter.Value);
            }

            // Department Decision Filter
            if (DepartmentDecisionFilter.HasValue)
            {
                query = query.Where(x =>
                    x.DepartmentDecision ==
                    DepartmentDecisionFilter.Value);
            }

            var projected = query
                .OrderByDescending(x => x.AppliedAt)
                .Select(x => new TrainingApplicationVM
                {
                    ApplicationID = x.ApplicationID,

                    StudentName = x.Student.Name,

                    StudentNumber = x.Student.StudentNumber,

                    DepartmentName = x.Student.Department.Name,

                    InstitutionName =
                        x.TrainingOpportunity.TrainingInstitution.Name,

                    OpportunityTitle =
                        x.TrainingOpportunity.Title,

                    AppliedAt = x.AppliedAt,

                    Status = x.Status,

                    DepartmentDecision =
                        x.DepartmentDecision,

                    UniversityAdminDecision =
                        x.UniversityAdminDecision
                });

            Applications =
                await PaginatedList<TrainingApplicationVM>
                .CreateAsync(projected, PageSize, PageIndex);
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var app = await _context.TrainingApplications
                .FirstOrDefaultAsync(x =>
                    x.ApplicationID == id);

            if (app == null)
                return NotFound();

            app.UniversityAdminDecision =
                TrainingApplication.Decision.Approved;

            app.Status = TrainingApplication.ApplicationStatus.UniversityApproved;

            app.UniversityAdminID = userId;

            app.UniversityAdminReviewedAt =
                DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var app = await _context.TrainingApplications
                .FirstOrDefaultAsync(x =>
                    x.ApplicationID == id);

            if (app == null)
                return NotFound();

            app.UniversityAdminDecision =
                TrainingApplication.Decision.Rejected;

            app.Status =
                TrainingApplication.ApplicationStatus.UniversityRejected;

            app.UniversityAdminID = userId;

            app.UniversityAdminReviewedAt =
                DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}