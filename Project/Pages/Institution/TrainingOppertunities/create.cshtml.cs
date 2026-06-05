using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;
using Project.Repository;

[Authorize(Roles = "InstitutionTrainingOfficer")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly TrainingInstitutionRepository _institutionRepo;
    private readonly UserManager<AspNetUser> _userManager;

    public CreateModel(ApplicationDbContext context, TrainingInstitutionRepository institutionRepo, UserManager<AspNetUser> userManager)
    {
        _context = context;
        this._institutionRepo = institutionRepo;
        this._userManager = userManager;
    }

    [BindProperty]
    public AddTrainingOpportunityVM Input { get; set; }
    public TrainingInstitution? CurrentInstitution { get; private set; }
    public int InstitutionId { get; set; }
    public string CurrentOfficerId { get; set; }
    public List<SelectListItem> Terms { get; set; }


    public async Task<ActionResult> OnGetAsync()
    {
        await LoadPageDataAsync();

        if (CurrentInstitution == null)
            return RedirectToPage("/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // التحقق من التواريخ

        if (Input.StartDate != default &&
            Input.StartDate.Date < DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(Input.StartDate),
                "لا يمكن اختيار تاريخ بداية في الماضي");
        }

        if (Input.EndDate != default &&
            Input.EndDate.Date < DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(Input.EndDate),
                "لا يمكن اختيار تاريخ نهاية في الماضي");
        }

        if (Input.EndDate <= Input.StartDate)
        {
            ModelState.AddModelError(
                nameof(Input.EndDate),
                "يجب أن يكون تاريخ النهاية بعد تاريخ البداية");
        }

        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync();
            return Page();
        }

        // جلب المستخدم الحالي

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToPage(
                "/Account/Login",
                new { area = "Identity" });
        }

        // جلب المؤسسة الحالية

        var scope = await _context.AspNetRoleScopes
            .FirstOrDefaultAsync(s =>
                s.UserID == user.Id);

        if (scope?.InstitutionID == null)
        {
            return RedirectToPage(
                "/Account/Login",
                new { area = "Identity" });
        }

        // إنشاء الفرصة

        var opportunity = new TrainingOpportunity
        {
            InstitutionID = scope.InstitutionID.Value,
            InstitutionOfficerID = user.Id,

            Title = Input.Title,
            Description = Input.Description,
            Capacity = Input.Capacity,

            StartDate = Input.StartDate,
            EndDate = Input.EndDate,

            Location = Input.Location,

            TermID = Input.TermId,

            RequestID = Input.RequestId,

            Status = (TrainingOpportunity.Opportunity)Input.Status,

            CreatedAt = DateTime.Now
        };

        _context.TrainingOpportunities.Add(opportunity);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "تمت إضافة الفرصة التدريبية بنجاح";

        return RedirectToPage("../AvailableOpportunities");
    }

    private async Task LoadPageDataAsync()
    {
        await LoadTermsAsync();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return;

        var scope = await _context.AspNetRoleScopes
            .FirstOrDefaultAsync(s => s.UserID == user.Id);

        if (scope?.InstitutionID == null)
            return;

        InstitutionId = scope.InstitutionID.Value;
        CurrentOfficerId = user.Id;

        CurrentInstitution =
            await _institutionRepo.GetByIdAsync(InstitutionId);
    }
    private async Task LoadTermsAsync()
    {
        //should move this to its repository
        Terms = await _context.TrainingTerms
            .Where(t => t.IsActive)
            .OrderByDescending(t => t.StartDate)
            .Select(t => new SelectListItem
            {
                Value = t.TermID.ToString(),
                Text = $"{t.Name} - {t.AcademicYear}"
            })
            .ToListAsync();
    }
}