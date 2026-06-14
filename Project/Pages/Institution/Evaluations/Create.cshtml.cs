using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;
using Project.Services;

namespace Project.Pages.Institution.Evaluations;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUploadFils _uploadFile;

    public CreateModel(
        ApplicationDbContext context,
        UserManager<AspNetUser> userManager,
        [FromKeyedServices("file")] IUploadFils uploadFile)
    {
        _context = context;
        _userManager = userManager;
        _uploadFile = uploadFile;
    }

    [BindProperty]
    public CreateEvaluationVM Input { get; set; } = new();

    [BindProperty]
    public IFormFile? EvaluationFile { get; set; }

    public string StudentName { get; set; } = "";
    public string OpportunityTitle { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(int placementId)
    {
        var placement = await _context.TrainingPlacements
            .Include(x => x.Student)
            .Include(x => x.TrainingOpportunity)
            .FirstOrDefaultAsync(x => x.PlacementID == placementId);

        if (placement == null)
            return NotFound();

        Input.PlacementID = placementId;

        StudentName = placement.Student.Name;
        OpportunityTitle = placement.TrainingOpportunity.Title;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var placement = await _context.TrainingPlacements
            .FirstOrDefaultAsync(x => x.PlacementID == Input.PlacementID);

        if (placement == null)
            return NotFound();

        string pdfPath = "";

        if (EvaluationFile != null)
        {
            pdfPath = _uploadFile.UploadFile(EvaluationFile,"StudentEvaluations");
        }


        var exists = await _context.StudentEvaluations.AnyAsync(x =>
            x.PlacementID == placement.PlacementID &&
            x.Type == StudentEvaluation.EvaluationType.InstitutionFinal);

        if (exists)
        {
            ModelState.AddModelError("", "تم إنشاء تقييم نهائي مسبقاً");
            return Page();
        }


        var user = await _userManager.GetUserAsync(User);

        var evaluation = new StudentEvaluation
        {
            PlacementID = placement.PlacementID,

            InstitutionSupervisorID = user?.Id,

            Type = StudentEvaluation.EvaluationType.InstitutionFinal,

            EvaluationDate = DateTime.Now,

            Score = Input.Score,
            MaxScore = Input.MaxScore,

            Notes = Input.Notes ?? "",

            EvaluationPdfPath = pdfPath,

            Status = StudentEvaluation.StudentEvaluationStatus.Submitted
        };

        _context.StudentEvaluations.Add(evaluation);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم حفظ التقييم بنجاح";

        return RedirectToPage(
            "/Institution/CurrentTraining/StudentDetails",
            new { placementId = Input.PlacementID });
    }
}