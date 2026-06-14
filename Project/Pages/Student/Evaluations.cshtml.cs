using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class EvaluationsModel : PageModel
    {
        private readonly StudentsRepository _studentsRepository;

        public EvaluationsModel(
            StudentsRepository studentsRepository)
        {
            _studentsRepository = studentsRepository;
        }

        public PaginatedList<StudentEvaluation> Evaluations { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EvaluationType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EvaluationStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EvaluatorType { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToPage(
                    "/Identity/Account/Login");

            var student =
                await _studentsRepository
                .GetStudentByUserId(userId);

            if (student == null)
                return Forbid();

            var query =
                _studentsRepository
                .GetStudentEvaluations(student.StudentID);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(x =>

                    x.Notes.Contains(SearchTerm)

                    ||

                    x.TrainingPlacement
                     .TrainingOpportunity
                     .Title
                     .Contains(SearchTerm));
            }

            if (EvaluationType.HasValue)
            {
                query = query.Where(x =>
                    (int)x.Type ==
                    EvaluationType.Value);
            }

            if (EvaluationStatus.HasValue)
            {
                query = query.Where(x =>
                    (int)x.Status ==
                    EvaluationStatus.Value);
            }

            // 1 = جامعة
            // 2 = مؤسسة

            if (EvaluatorType == 1)
            {
                query = query.Where(x =>
                    x.UniversitySupervisorID != null);
            }

            if (EvaluatorType == 2)
            {
                query = query.Where(x =>
                    x.InstitutionSupervisorID != null);
            }

            query = query
                .OrderByDescending(x =>
                    x.EvaluationDate);

            Evaluations =
                await PaginatedList<StudentEvaluation>
                .CreateAsync(
                    query,
                    PageSize,
                    PageIndex);

            return Page();
        }

        public async Task<IActionResult>
            OnGetOpenPdf(int evaluationId)
        {
            var evaluation =
                await _studentsRepository
                .GetStudentEvaluation(
                    evaluationId);

            if (evaluation == null)
                return NotFound();

            if (string.IsNullOrEmpty(
                evaluation.EvaluationPdfPath))
            {
                return NotFound();
            }

            var provider =
                new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(
                evaluation.EvaluationPdfPath,
                out string? contentType))
            {
                contentType =
                    "application/octet-stream";
            }

            var physicalPath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    evaluation
                    .EvaluationPdfPath
                    .TrimStart('/'));

            return PhysicalFile(
                physicalPath,
                contentType);
        }
    }
}