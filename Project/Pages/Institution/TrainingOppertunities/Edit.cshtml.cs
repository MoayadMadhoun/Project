using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Institution.ViewModels;


namespace Project.Pages.Institution.AvailableOpportunities
{
    [Authorize(Roles = "InstitutionTrainingOfficer")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;

        public EditModel(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public AddTrainingOpportunityVM Input { get; set; }

        public TrainingInstitution? CurrentInstitution { get; set; }

        public SelectList Terms { get; set; }

        public SelectList Specialties { get; set; }

        public List<Skill> Skills { get; set; } = [];

        public int OpportunityId { get; set; }

        public DateTime CreatedAt { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage("/Account/Login",
                    new { area = "Identity" });

            var opportunity = await _context.TrainingOpportunities
                .Include(x => x.TrainingInstitution)
                .Include(x => x.OpportunitySpecialties)
                .Include(x => x.OpportunitySkills)
                .FirstOrDefaultAsync(x =>
                    x.OpportunityID == id);

            if (opportunity == null)
                return NotFound();

            CurrentInstitution =
                opportunity.TrainingInstitution;

            OpportunityId =
                opportunity.OpportunityID;

            CreatedAt =
                opportunity.CreatedAt;

            Input = new AddTrainingOpportunityVM
            {
                Title = opportunity.Title,
                Description = opportunity.Description ?? "",
                Capacity = opportunity.Capacity,
                StartDate = opportunity.StartDate,
                EndDate = opportunity.EndDate,
                Location = opportunity.Location ?? "",
                TermId = opportunity.TermID,
                RequestId = opportunity.RequestID,

                Status =
                    (AddTrainingOpportunityVM.Opportunity)
                    ((int)opportunity.Status),

                SelectedSpecialties =
                    opportunity.OpportunitySpecialties
                    .Select(x => x.SpecialtyID)
                    .ToList(),

                SelectedSkills =
                    opportunity.OpportunitySkills
                    .Select(x => x.SkillID)
                    .ToList(),

                SkillTypes =
                    opportunity.OpportunitySkills
                    .ToDictionary(
                        x => x.SkillID,
                        x => x.IsRequired)
            };

            await LoadPageDataAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadPageDataAsync();
                return Page();
            }

            var opportunity =
                await _context.TrainingOpportunities

                .Include(x => x.OpportunitySpecialties)

                .Include(x => x.OpportunitySkills)

                .FirstOrDefaultAsync(x =>
                    x.OpportunityID == id);

            if (opportunity == null)
                return NotFound();

            opportunity.Title = Input.Title;
            opportunity.Description = Input.Description;
            opportunity.Capacity = Input.Capacity;
            opportunity.StartDate = Input.StartDate;
            opportunity.EndDate = Input.EndDate;
            opportunity.Location = Input.Location;
            opportunity.TermID = Input.TermId;
            opportunity.RequestID = Input.RequestId;

            opportunity.Status =
                (TrainingOpportunity.Opportunity)
                ((int)Input.Status);

            // حذف التخصصات القديمة

            _context.OpportunitySpecialties
                .RemoveRange(
                    opportunity.OpportunitySpecialties);

            // حذف المهارات القديمة

            _context.OpportunitySkills
                .RemoveRange(
                    opportunity.OpportunitySkills);

            await _context.SaveChangesAsync();

            // إضافة التخصصات الجديدة

            if (Input.SelectedSpecialties.Any())
            {
                _context.OpportunitySpecialties.AddRange(

                    Input.SelectedSpecialties.Select(x =>

                        new OpportunitySpecialty
                        {
                            OpportunityID =
                                opportunity.OpportunityID,

                            SpecialtyID = x
                        }));
            }

            // إضافة المهارات الجديدة

            if (Input.SelectedSkills.Any())
            {
                _context.OpportunitySkills.AddRange(

                    Input.SelectedSkills.Select(skillId =>

                        new OpportunitySkill
                        {
                            OpportunityID =
                                opportunity.OpportunityID,

                            SkillID = skillId,

                            IsRequired =
                                Input.SkillTypes
                                .TryGetValue(
                                    skillId,
                                    out var required)

                                && required
                        }));
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "تم تعديل الفرصة التدريبية بنجاح";

            return RedirectToPage("/Institution/AvailableOpportunities");
        }

        private async Task LoadPageDataAsync()
        {
            Terms = new SelectList(

                await _context.TrainingTerms
                    .OrderBy(x => x.Name)
                    .ToListAsync(),

                "TermID",
                "Name");

            Specialties = new SelectList(

                await _context.Specialties
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Name)
                    .ToListAsync(),

                "SpecialtyID",
                "Name");

            Skills = await _context.Skills
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}