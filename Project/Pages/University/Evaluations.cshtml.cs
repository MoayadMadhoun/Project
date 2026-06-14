using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using System.Security.Claims;

namespace Project.Pages.University
{
    public class EvaluationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public EvaluationsModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public PaginatedList<StudentEvaluation> Evaluations { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EvaluationTypeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EvaluationStatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public int? StudentId { get; set; }
        public Models.Student? SelectedStudent { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login",
                    new { area = "Identity" });

            var scope = await _context.AspNetRoleScopes
                .FirstOrDefaultAsync(x =>
                    x.UserID == user.Id &&
                    x.IsActive);

            if (scope == null)
                return RedirectToPage("/Index");

            var query = _context.StudentEvaluations

     .Include(e => e.TrainingPlacement)
         .ThenInclude(p => p.Student)
             .ThenInclude(s => s.Department)

     .Include(e => e.TrainingPlacement)
         .ThenInclude(p => p.Student)
             .ThenInclude(s => s.Specialty)

     .Include(e => e.TrainingPlacement)
         .ThenInclude(p => p.TrainingInstitution)

     .Include(e => e.TrainingPlacement)
         .ThenInclude(p => p.TrainingOpportunity)

     .Include(e => e.UniversitySupervisor)

     .Where(e =>
         e.TrainingPlacement.Student.UniversityID ==
         scope.UniversityID.Value)

     .AsNoTracking()

     .AsQueryable();

            if (StudentId.HasValue)
            {
                query = query.Where(e =>
                    e.TrainingPlacement.StudentID ==
                    StudentId.Value);
            }
            if (StudentId.HasValue)
            {
                SelectedStudent = await _context.Students
                    .FirstOrDefaultAsync(s =>
                        s.StudentID == StudentId.Value);
            }
            /*
             * Department Head
             */

            if (User.IsInRole("DepartmentHead"))
            {
                if (scope.DepartmentID.HasValue)
                {
                    query = query.Where(e =>
                        e.TrainingPlacement.Student.DepartmentID
                        == scope.DepartmentID.Value);
                }
            }

            /*
             * Search
             */

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(e =>
                    e.TrainingPlacement.Student.Name
                    .Contains(SearchTerm));
            }

            /*
             * Evaluation Type
             */

            if (EvaluationTypeId.HasValue)
            {
                query = query.Where(e =>
                    (int)e.Type ==
                    EvaluationTypeId.Value);
            }

            /*
             * Status
             */

            if (EvaluationStatusId.HasValue)
            {
                query = query.Where(e =>
                    (int)e.Status ==
                    EvaluationStatusId.Value);
            }

            query = query
                .OrderByDescending(e => e.CreatedAt);

            Evaluations =
                await PaginatedList<StudentEvaluation>
                .CreateAsync(
                    query,
                    PageSize,
                    PageIndex);

            return Page();
        }

        public async Task<IActionResult> OnGetOpenFile(int id)
        {
            var evaluation =
                await _context.StudentEvaluations
                .FirstOrDefaultAsync(x =>
                    x.EvaluationID == id);

            if (evaluation == null)
                return NotFound();

            if (string.IsNullOrEmpty(
                evaluation.EvaluationPdfPath))
                return NotFound();

            var provider =
                new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(
                evaluation.EvaluationPdfPath,
                out string? contentType))
            {
                contentType =
                    "application/octet-stream";
            }

            return PhysicalFile(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    evaluation.EvaluationPdfPath.TrimStart('/')),
                contentType);
        }
    }
}