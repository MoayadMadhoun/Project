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

        Terms = await _context.TrainingTerms
            .Select(t => new SelectListItem
            {
                Value = t.TermID.ToString(),
                Text = t.Name
            })
            .ToListAsync();

        try
        {
            if (User == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var scope = await _context.AspNetRoleScopes.FirstOrDefaultAsync(s => s.UserID == user.Id);

            if (scope == null)
            {

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (scope.InstitutionID == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

           InstitutionId = scope.InstitutionID.Value;

            CurrentInstitution = await _institutionRepo.GetByIdAsync(InstitutionId);
            if (CurrentInstitution == null)
            {
                return RedirectToPage("/Index");
            }
            CurrentOfficerId = user.Id;

            return Page();

        }
        catch (Exception ex)
        {
            return RedirectToPage("/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        var scope = await _context.AspNetRoleScopes
            .FirstOrDefaultAsync(s => s.UserID == user.Id);

        if (scope?.InstitutionID == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

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

        return RedirectToPage("../AvailableOpportunities");
    }
}