using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repository;

namespace Project.Pages.Institution
{
    public class EvaluationsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TrainingInstitutionRepository _institutionRepository;
        private readonly UserManager<AspNetUser> _userManager;

        public EvaluationsModel(
            ApplicationDbContext dbContext,
            TrainingInstitutionRepository institutionRepository,
            UserManager<AspNetUser> userManager)
        {
            _dbContext = dbContext;
            _institutionRepository = institutionRepository;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedEvaluationTypeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedEvaluationStatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int? StudentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PlacementId { get; set; }
        public string CurrentStudentName { get; set; } = "";

        public PaginatedList<StudentEvaluation> Evaluations { get; set; }

        public TrainingInstitution? CurrentInstitution { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                    return RedirectToPage("/Account/Login", new { area = "Identity" });

                var scope = await _dbContext.AspNetRoleScopes
                    .FirstOrDefaultAsync(x =>
                        x.UserID == user.Id &&
                        x.IsActive);

                if (scope?.InstitutionID == null)
                    return RedirectToPage("/Account/Login", new { area = "Identity" });

                int institutionId = scope.InstitutionID.Value;

                CurrentInstitution =
                    await _institutionRepository.GetByIdAsync(institutionId);

                if (CurrentInstitution == null)
                    return RedirectToPage("/Index");

                var query =
                    _institutionRepository.GetEvaluationsForInstitution(institutionId);
                if (StudentId.HasValue)
                {
                    query = query.Where(e =>
                        e.TrainingPlacement.StudentID == StudentId.Value);
                }

                if (PlacementId.HasValue)
                {
                    query = query.Where(e =>
                        e.PlacementID == PlacementId.Value);
                }

                if (StudentId.HasValue)
                {
                    CurrentStudentName = await _dbContext.Students
                        .Where(s => s.StudentID == StudentId.Value)
                        .Select(s => s.Name)
                        .FirstOrDefaultAsync() ?? "";
                }
                // البحث باسم الطالب

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(e =>
                        e.TrainingPlacement.Student.Name.Contains(SearchTerm));
                }

                // فلترة الطالب القادم من صفحة تفاصيل الطالب

                if (StudentId.HasValue)
                {
                    query = query.Where(e =>
                        e.TrainingPlacement.StudentID == StudentId.Value);
                }

                // فلترة التدريب الحالي

                if (PlacementId.HasValue)
                {
                    query = query.Where(e =>
                        e.PlacementID == PlacementId.Value);
                }

                // فلترة النوع

                if (SelectedEvaluationTypeId > 0)
                {
                    query = query.Where(e =>
                        (int)e.Type == SelectedEvaluationTypeId);
                }

                // فلترة الحالة

                if (SelectedEvaluationStatusId > 0)
                {
                    query = query.Where(e =>
                        (int)e.Status == SelectedEvaluationStatusId);
                }

                query = query
                    .OrderByDescending(e => e.EvaluationDate);

                Evaluations =
                    await PaginatedList<StudentEvaluation>
                    .CreateAsync(query, PageSize, PageIndex);

                return Page();
            }
            catch
            {
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnGetOpenFileAsync(int id)
        {
            var evaluation = await _dbContext.StudentEvaluations
                .FirstOrDefaultAsync(x => x.EvaluationID == id);

            if (evaluation == null ||
                string.IsNullOrEmpty(evaluation.EvaluationPdfPath))
            {
                return NotFound();
            }

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(
                evaluation.EvaluationPdfPath,
                out string? contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    evaluation.EvaluationPdfPath.TrimStart('/')),
                contentType);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var evaluation =
                await _dbContext.StudentEvaluations
                .FirstOrDefaultAsync(x => x.EvaluationID == id);

            if (evaluation == null)
                return NotFound();

            _dbContext.StudentEvaluations.Remove(evaluation);

            await _dbContext.SaveChangesAsync();

            TempData["Success"] =
                "تم حذف التقييم بنجاح";

            return RedirectToPage();
        }
    }
}