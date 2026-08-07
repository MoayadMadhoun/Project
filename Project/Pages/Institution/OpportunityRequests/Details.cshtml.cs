using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Institution.OpportunityRequests
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public DetailsModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public TrainingOpportunityRequest Request { get; set; } = default!;

        public bool HasResponded { get; set; }

        public bool IsInvitation { get; set; }

        public OpportunityRequestInstitution? Invitation { get; set; }

        public async Task<IActionResult> OnGetAsync(int requestId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var institution = await _context.TrainingInstitutions
                .FirstOrDefaultAsync(x => x.UserID == user.Id);

            if (institution == null)
                return NotFound();

            Request = await _context.TrainingOpportunityRequests

                .Include(x => x.University)

                .Include(x => x.Department)

                .Include(x => x.UniversityAdmin)

                .Include(x => x.DepartmentHead)

                .Include(x => x.TrainingTerm)

                .Include(x => x.Specialties)
                    .ThenInclude(x => x.Specialty)

                .Include(x => x.Skills)
                    .ThenInclude(x => x.Skill)

                .FirstOrDefaultAsync(x => x.RequestID == requestId);

            if (Request == null)
                return NotFound();

            Invitation = await _context.OpportunityRequestInstitutions
                .FirstOrDefaultAsync(x =>
                    x.RequestID == requestId &&
                    x.InstitutionID == institution.InstituationID);

            IsInvitation = Invitation != null;

            HasResponded = await _context.TrainingOpportunities
                .AnyAsync(x =>
                    x.RequestID == requestId &&
                    x.InstitutionID == institution.InstituationID);

            // أول مشاهدة للدعوة
            if (Invitation != null &&
                Invitation.Status == OpportunityRequestInstitution.OpportunityRequestInstitutionStatus.Invited)
            {
                Invitation.Status =
                    OpportunityRequestInstitution.OpportunityRequestInstitutionStatus.Viewed;

                await _context.SaveChangesAsync();
            }

            return Page();
        }
    }
}