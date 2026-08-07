using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Institution.Evaluations;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public StudentEvaluation Evaluation { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Evaluation = await _context.StudentEvaluations
            .Include(e => e.TrainingPlacement)
                .ThenInclude(p => p.Student)
            .Include(e => e.TrainingPlacement)
                .ThenInclude(p => p.TrainingOpportunity)
            .FirstOrDefaultAsync(e => e.EvaluationID == id);

        if (Evaluation == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnGetOpenFileAsync(int id)
    {
        var evaluation = await _context.StudentEvaluations
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


    public async Task<IActionResult> OnPostAsync(int id)
    {
        var evaluation = await _context.StudentEvaluations
            .FirstOrDefaultAsync(e => e.EvaluationID == id);

        if (evaluation == null)
            return NotFound();

        _context.StudentEvaluations.Remove(evaluation);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حذف التقييم بنجاح";

        return RedirectToPage("/Institution/Evaluations");
    }
}