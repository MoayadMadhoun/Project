using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Pages.Institution.OpportunityRequests
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // جميع الطلبات المنشورة
        public IList<TrainingOpportunityRequest> PublicRequests { get; set; }
            = new List<TrainingOpportunityRequest>();

        // الطلبات الموجهة للمؤسسة
        public IList<OpportunityRequestInstitution> MyInvitations { get; set; }
            = new List<OpportunityRequestInstitution>();

        // إحصائيات
        public int PublicRequestsCount { get; set; }

        public int InvitationCount { get; set; }

        public int RespondedCount { get; set; }

        public int NewInvitationsCount { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return;

            var institution = await _context.TrainingInstitutions
                .FirstOrDefaultAsync(x => x.UserID == user.Id);

            if (institution == null)
                return;

            int institutionId = institution.InstituationID;

            //---------------------------------------------
            // جميع الطلبات المنشورة
            //---------------------------------------------

            PublicRequests = await _context.TrainingOpportunityRequests

                .Include(x => x.University)

                .Include(x => x.TrainingTerm)

                .Include(x => x.Specialties)
                    .ThenInclude(x => x.Specialty)

                .Include(x => x.Skills)
                    .ThenInclude(x => x.Skill)

                .Where(x => x.Status ==
                    TrainingOpportunityRequest.RequestStatus.Published)

                .OrderByDescending(x => x.CreatedAt)

                .ToListAsync();

            //---------------------------------------------
            // الدعوات الموجهة للمؤسسة
            //---------------------------------------------

            MyInvitations = await _context.OpportunityRequestInstitutions
                .Include(x => x.Request)
                    .ThenInclude(x => x.University)
                .Include(x => x.Request)
                    .ThenInclude(x => x.TrainingTerm)
                .Where(x => x.InstitutionID == institutionId)
                .OrderByDescending(x => x.SentAt)
                .ToListAsync();

            //---------------------------------------------
            // الإحصائيات
            //---------------------------------------------

            PublicRequestsCount = PublicRequests.Count;

            InvitationCount = MyInvitations.Count;

            NewInvitationsCount = MyInvitations.Count(x =>
                x.Status ==
                OpportunityRequestInstitution.OpportunityRequestInstitutionStatus.Invited);

            RespondedCount = MyInvitations.Count(x =>
                x.Status ==
                OpportunityRequestInstitution.OpportunityRequestInstitutionStatus.Reponded);
        }
    }
}