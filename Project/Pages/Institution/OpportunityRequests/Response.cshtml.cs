using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;

namespace Project.Pages.Institution.OpportunityRequests
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class ResponseModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public ResponseModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public CreateOpportunityFromRequestVM Opportunity { get; set; } = new();

        public TrainingOpportunityRequest Request { get; set; } = default!;
        public List<OpportunitySpecialtyVM> Specialties { get; set; }

        public async Task<IActionResult> OnGetAsync(int requestId)
        {
            var user = await _userManager.GetUserAsync(User);

            var institution = await _context.TrainingInstitutions.FirstOrDefaultAsync(x => x.UserID == user.Id);

            if (institution == null)
                return NotFound();

            Request = await _context.TrainingOpportunityRequests
                .Include(x => x.TrainingTerm)
                .Include(x => x.Specialties)
                    .ThenInclude(x => x.Specialty)
                .Include(x => x.Skills)
                    .ThenInclude(x => x.Skill)
                .Include(x => x.University)
                .FirstOrDefaultAsync(x => x.RequestID == requestId);

            if (Request == null)
                return NotFound();

            bool exists = await _context.TrainingOpportunities
                .AnyAsync(x =>
                    x.RequestID == requestId &&
                    x.InstitutionID == institution.InstituationID);

            if (exists)
                return RedirectToPage("/Institution/OpportunityRequests/Details",
                    new { requestId });

            Opportunity.RequestID = Request.RequestID;

            Opportunity.Title = Request.Title;

            Opportunity.Description = Request.Description;

            Opportunity.Capacity = Request.RequestedSeats;

            Opportunity.StartDate = (DateTime)Request.PreferredStartDate;

            Opportunity.EndDate = (DateTime)Request.PreferredEndDate;

            Opportunity.TermID = Request.TermID;

            Opportunity.Specialties = Request.Specialties
            .Select(x => new OpportunitySpecialtyVM
            {
                SpecialtyID = x.SpecialtyID,
                SpecialtyName = x.Specialty.Name,
                Selected = true
            })
            .ToList();

            Opportunity.Skills = Request.Skills
                .Select(x => new OpportunitySkillVM
                {
                    SkillID = x.SkillID,
                    SkillName = x.Skill.Name,
                    IsRequired = x.IsRequired,
                    Selected = true
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Challenge();

            var institution = await _context.TrainingInstitutions
                .FirstOrDefaultAsync(x => x.UserID == user.Id);

            if (institution == null)
                return NotFound();

            var request = await _context.TrainingOpportunityRequests

                .Include(x => x.Specialties)

                .Include(x => x.Skills)

                .FirstOrDefaultAsync(x => x.RequestID == Opportunity.RequestID);

            if (request == null)
                return NotFound();

            bool exists = await _context.TrainingOpportunities
                .AnyAsync(x =>
                    x.RequestID == request.RequestID &&
                    x.InstitutionID == institution.InstituationID);

            if (exists)
            {
                ModelState.AddModelError("", "لقد قمت بالاستجابة لهذا الطلب مسبقاً.");

                return Page();
            }

            //----------------------------------------------------
            // إنشاء الفرصة
            //----------------------------------------------------

            var trainingOpportunity = new TrainingOpportunity
            {
                RequestID = request.RequestID,

                InstitutionID = institution.InstituationID,

                InstitutionOfficerID = user.Id,

                TermID = Opportunity.TermID,

                Title = Opportunity.Title,

                Description = Opportunity.Description,

                Capacity = Opportunity.Capacity,

                StartDate = Opportunity.StartDate,

                EndDate = Opportunity.EndDate,

                Location = Opportunity.Location,

                Status = TrainingOpportunity.Opportunity.Open,

                CreatedAt = DateTime.UtcNow
            };

            _context.TrainingOpportunities.Add(trainingOpportunity);

            await _context.SaveChangesAsync();

            //----------------------------------------------------
            // نسخ التخصصات
            //----------------------------------------------------

            foreach (var specialty in Opportunity.Specialties.Where(x => x.Selected))
            {
                _context.OpportunitySpecialties.Add(
                    new OpportunitySpecialty
                    {
                        OpportunityID = trainingOpportunity.OpportunityID,
                        SpecialtyID = specialty.SpecialtyID
                    });
            }

            //----------------------------------------------------
            // نسخ المهارات
            //----------------------------------------------------

            foreach (var skill in Opportunity.Skills.Where(x => x.Selected))
            {
                _context.OpportunitySkills.Add(
                    new OpportunitySkill
                    {
                        OpportunityID = trainingOpportunity.OpportunityID,

                        SkillID = skill.SkillID,

                        IsRequired = skill.IsRequired
                    });
            }

            //----------------------------------------------------
            // تحديث حالة الدعوة
            //----------------------------------------------------

            var invitation = await _context.OpportunityRequestInstitutions

                .FirstOrDefaultAsync(x =>
                    x.RequestID == request.RequestID &&
                    x.InstitutionID == institution.InstituationID);

            if (invitation != null)
            {
                invitation.Status =
                    OpportunityRequestInstitution
                    .OpportunityRequestInstitutionStatus.Reponded;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Institution/OpportunityRequests/Index");
        }
    }
}
