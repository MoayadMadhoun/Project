using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;
using Project.Services;

namespace Project.Pages.Institution.Evaluations;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IUploadFils _uploadFile;

    public EditModel(
        ApplicationDbContext context,
        [FromKeyedServices("file")] IUploadFils uploadFile)
    {
        _context = context;
        _uploadFile = uploadFile;
    }

    [BindProperty]
    public CreateEvaluationVM Input { get; set; } = new();

    [BindProperty]
    public IFormFile? EvaluationFile { get; set; }

    public StudentEvaluation Evaluation { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var evaluation = await _context.StudentEvaluations
            .Include(x => x.TrainingPlacement)
                .ThenInclude(x => x.Student)
            .Include(x => x.TrainingPlacement)
                .ThenInclude(x => x.TrainingOpportunity)
            .FirstOrDefaultAsync(x => x.EvaluationID == id);

        if (evaluation == null)
            return NotFound();

        Evaluation = evaluation;

        Input = new CreateEvaluationVM
        {
            PlacementID = evaluation.PlacementID,
            Type = evaluation.Type,
            EvaluationDate = evaluation.EvaluationDate,
            Score = evaluation.Score,
            MaxScore = evaluation.MaxScore,
            Notes = evaluation.Notes
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var evaluation = await _context.StudentEvaluations
            .FirstOrDefaultAsync(x => x.EvaluationID == id);

        if (evaluation == null)
            return NotFound();

        evaluation.Type = Input.Type;
        evaluation.EvaluationDate = Input.EvaluationDate;
        evaluation.Score = Input.Score;
        evaluation.MaxScore = Input.MaxScore;
        evaluation.Notes = Input.Notes;

        if (EvaluationFile != null)
        {
            evaluation.EvaluationPdfPath =
                _uploadFile.UploadFile(EvaluationFile, "Evaluations");
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تعديل التقييم بنجاح";

        return RedirectToPage("/Institution/Evaluations");
    }
}