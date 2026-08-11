using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories;
using Project.Models.Enums;
using Project.Services;
using System.Security.Claims;

namespace Project.Pages.Student.Applications
{
    public class CreateModel : PageModel
    {
        private readonly StudentsRepository studentsRepository;

        private readonly ApplicationDbContext _contex;
        private readonly INotificationService _notificationService;

        public CreateModel(StudentsRepository StudentsRepository, ApplicationDbContext contex, INotificationService notificationService)
        {
            studentsRepository = StudentsRepository;
            _contex = contex;
            _notificationService = notificationService;
        }

        [BindProperty]
        public string? studentNotes { get; set; }

        [BindProperty]
        public bool IsAgree { get; set; } = false;

        public TrainingOpportunity? opportunity { get; set; }
        public Project.Models.Student? student { get; set; }


        [BindProperty(SupportsGet = true)]
        public int OpportunityId { get; set; }

        private async Task<int> LoadData()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

             student = await studentsRepository.GetStudentByUserId(userId);

            if (student == null) return 1;

            opportunity = await _contex.TrainingOpportunities.Include(to => to.TrainingInstitution).
            Include(to => to.TrainingPlacement).Include(to => to.TrainingTerm).
            FirstOrDefaultAsync(to => to.OpportunityID == OpportunityId);

            if (opportunity == null) return 2;

            return 3;
        }
        public async Task<IActionResult> OnGet()
        {
            var result = await LoadData();

            if (result == 1) return RedirectToPage("/Identity/Account/Login");

            if (result == 2) return NotFound();

            return Page();

        }
        public async Task<IActionResult> OnPost()
        {

            var result = await LoadData();

            if (result == 1) return RedirectToPage("/Identity/Account/Login");

            if (result == 2) return NotFound();

            if (!IsAgree)
            {

                ModelState.AddModelError(nameof(IsAgree), " يجب عليك الموافقة على الشروط لاكمال التقديم على الفرصة التدريبة  ⚠️ ");

                return Page();

            }
            bool alreadyApplied = _contex.TrainingApplications.Any(a => a.StudentID == student.StudentID &&
              a.OpportunityID == opportunity.OpportunityID);

            if (alreadyApplied)
            {
                ModelState.AddModelError("", "لقد قمت بالتقديم مسبقاً");
                return Page();
            }

            var newTrainingApplication = new TrainingApplication
            {

                OpportunityID = opportunity.OpportunityID,
                Status = Models.TrainingApplication.ApplicationStatus.Submitted,
                StudentID = student.StudentID,
                Student = student,
                TrainingOpportunity = opportunity
            };


            if (!string.IsNullOrWhiteSpace(studentNotes)) newTrainingApplication.StudentNotes = studentNotes;

            await studentsRepository.AddTrainingApplication(newTrainingApplication);

            await _notificationService.NotifyDepartmentHeadsAsync(
                student.DepartmentID,
                "طلب تدريب جديد",
                $"قام الطالب {student.Name} بتقديم طلب تدريب جديد على فرصة {opportunity.Title}. يرجى مراجعة الطلب.",
                NotificationType.Application,
                "/University/DepartmentHeadTrainingRequests",
                "fa-solid fa-file-circle-plus",
                student.UserID,
                newTrainingApplication.ApplicationID.ToString(),
                nameof(TrainingApplication));

            return RedirectToPage("/Student/AvaliabelOpportunities");

        }
    }
}
