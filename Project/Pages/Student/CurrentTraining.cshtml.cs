using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class CurrentTrainingModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentsRepository studentsRepository;

        public TrainingPlacement? CurrentPlacement { get; set; }
        public IList<StudentReport> RecentReports { get; set; }
            = new List<StudentReport>();

        public IList<StudentEvaluation> Evaluations { get; set; }
            = new List<StudentEvaluation>();

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int ExcusedCount { get; set; }

        public CurrentTrainingModel(ApplicationDbContext context , StudentsRepository studentsRepository)
        {
           _context = context;
            this.studentsRepository = studentsRepository;
        }
        public string GetTrainingDuration(TrainingPlacement? placement)
        {
            if (placement is null)
                return "-";

            int days =
                (placement.EndDate - placement.StartDate).Days;

            if (days < 30)
                return $"{days} يوم";

            int months = days / 30;

            return $"{months} شهر";
        }
        public string TrainingDuration { get; set; }
        public async Task <IActionResult> OnGet()
            
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) { return RedirectToPage("/Identity/Account/Login"); }
            var student = await studentsRepository.GetStudentByUserId(userId);
            if (student is null) return Forbid();

            CurrentPlacement = await studentsRepository.GetTrainingPlacementByStudentId(student.StudentID);
             TrainingDuration = GetTrainingDuration(CurrentPlacement);

            RecentReports = CurrentPlacement.StudentReports
                .OrderByDescending(x => x.SubmittedAt)
                .Take(5)
                .ToList();

            Evaluations = CurrentPlacement.StudentEvaluations
                .OrderByDescending(x => x.EvaluationDate)
                .ToList();

            PresentCount = CurrentPlacement.AttendanceRecords
                .Count(x => x.Status == AttendanceRecord.AttendanceStatus.Present);

            AbsentCount = CurrentPlacement.AttendanceRecords
                .Count(x => x.Status == AttendanceRecord.AttendanceStatus.Absent);

            LateCount = CurrentPlacement.AttendanceRecords
                .Count(x => x.Status == AttendanceRecord.AttendanceStatus.Late);

            ExcusedCount = CurrentPlacement.AttendanceRecords
                .Count(x => x.Status == AttendanceRecord.AttendanceStatus.Excused);
            return Page();
        }
    }
}
