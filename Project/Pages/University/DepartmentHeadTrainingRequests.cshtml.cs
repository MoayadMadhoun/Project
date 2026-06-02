using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Models.Enums;
using Project.ViewModel;

namespace Project.Pages.University
{
    public class DepartmentHeadTrainingRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public DepartmentHeadTrainingRequestsModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public TrainingApplication.ApplicationStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public PaginatedList<TrainingApplicationVM> Applications { get; set; } = new(new List<TrainingApplicationVM>(), 1, 0, 10);
    

        public SelectList StatusList { get; set; }

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            var roleScope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x => x.UserID == userId && x.DepartmentID != null && x.IsActive);

            if (roleScope == null)
            {
                Applications = new PaginatedList<TrainingApplicationVM>(
                    new List<TrainingApplicationVM>(), 1, 0, PageSize);

                return;
            }

            var departmentId = roleScope.DepartmentID;

            var query = _context.TrainingApplications
                .Include(x => x.Student)
                .Include(x => x.TrainingOpportunity)
                    .ThenInclude(o => o.TrainingInstitution)
                .Where(x => x.Student.DepartmentID == departmentId)
                .AsQueryable();

            //  Search
            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(x =>
                    x.Student.Name.Contains(Search) ||
                    x.Student.StudentNumber.Contains(Search) ||
                    x.TrainingOpportunity.TrainingInstitution.Name.Contains(Search));
            }

            //  Status Filter
            if (StatusFilter.HasValue)
            {
                query = query.Where(x => x.Status == StatusFilter);
            }

            //  Projection
            var projected = query.Select(x => new TrainingApplicationVM
            {
                ApplicationID = x.ApplicationID,
                StudentName = x.Student.Name,
                StudentNumber = x.Student.StudentNumber,
                InstitutionName = x.TrainingOpportunity.TrainingInstitution.Name,
                AppliedAt = x.AppliedAt,
                Status = x.Status,
                DepartmentDecision = x.DepartmentDecision
            });

            //  Pagination
            Applications = await PaginatedList<TrainingApplicationVM>
                .CreateAsync(projected, PageSize, PageIndex);

            //  Enum dropdown
            StatusList = EnumExtensions.GetEnumSelectList<TrainingApplication.ApplicationStatus>();
        }







        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var app = await _context.TrainingApplications
                .FirstOrDefaultAsync(x => x.ApplicationID == id);

            if (app == null) return NotFound();

            app.DepartmentDecision = TrainingApplication.Decision.Approved;
            app.Status = TrainingApplication.ApplicationStatus.DepartmentApproved;
            app.DepartmentHeadID = userId;
            app.DepartmentReviewedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var app = await _context.TrainingApplications
                .FirstOrDefaultAsync(x => x.ApplicationID == id);

            if (app == null) return NotFound();

            app.DepartmentDecision = TrainingApplication.Decision.Rejected;
            app.Status = TrainingApplication.ApplicationStatus.DepartmentRejected;
            app.DepartmentHeadID = userId;
            app.DepartmentReviewedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }



    }
}

