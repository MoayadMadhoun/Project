using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Evaluation;
using Project.Extensions;
using System.Security.Claims;
using Project.Repositories;
using Project.Data;
using Project.Models;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Project.Pages.Student
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentsRepository _studentRepo;

        [BindProperty(SupportsGet = true)]


        public string? search { get; set; }
        public Project.Models.Student? student { get; set; }

        public PaginatedList<TrainingApplication>? TrainingApplication { get; set; }

        public string? userId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;
        [BindProperty(SupportsGet = true)]
        public int pageIndex { get; set; } = 1;
        public bool ProfileStatus { get; set; }
        public int OpportunitiesCount { get; set; }
        public int AttendanceRate { get; set; }
        public enum TrainingStatusEnum
        {
            Active,
            Inactive
        }
        public int TotalApplication { get; set; }
        public TrainingStatusEnum statusTrain { get; set; }
        public int CurrentCount => TrainingApplication?.Count() ?? 0;
        public int TotalCount => TrainingApplication?.TotalCount ?? 0;
        public IndexModel(ApplicationDbContext context, StudentsRepository studentRepo)
        {
            _context = context;
            _studentRepo = studentRepo;
        }
        private async Task<int> CalculateAttendanceRate(int studentId)
        {
            int allAttendanceRecord = 0; int PresentAttendanceRecord = 0;
            allAttendanceRecord = await _context.AttendanceRecords.
              Where(a => a.TrainingPlacement.StudentID == studentId).CountAsync();
            if (allAttendanceRecord == 0) return 0;
            PresentAttendanceRecord = await _context.AttendanceRecords.
             Where(a => a.TrainingPlacement.StudentID == studentId &&
             (a.Status == AttendanceRecord.AttendanceStatus.Present || a.Status == AttendanceRecord.AttendanceStatus.Late)).CountAsync();
            return (int)(((double)PresentAttendanceRecord / allAttendanceRecord) * 100);

        }
        //To check whether the student is actually participating in a training or not.
        private async Task<bool> HasActiveTrainingPlacement(int studentId)
        {
            return await _context.TrainingPlacements
                .AnyAsync(tp => tp.StudentID == studentId);

        }

        public async Task<IActionResult> OnGet()
        {

            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) { return RedirectToPage("/Identity/Account/Login"); }
           
            student = await _studentRepo.GetStudentByUserId(userId);
            if (student == null) return  Forbid();

            AttendanceRate = await CalculateAttendanceRate(student.StudentID);

            ProfileStatus = await _studentRepo.GetProfileStatus(student.StudentID);

            var trainingApplication =   _studentRepo.GetTrainingApplications(student.StudentID).Where(
               ta => ta.Status != Models.TrainingApplication.ApplicationStatus.Withdrawn);
             TotalApplication = await trainingApplication.CountAsync() ;
            if (!string.IsNullOrEmpty(search))
            {

                trainingApplication = trainingApplication.Where(ta => EF.Functions.Like(ta.TrainingOpportunity.Title, $"%{search}%"));
            }


            TrainingApplication = await PaginatedList<TrainingApplication>.CreateAsync(trainingApplication, PageSize, pageIndex);

            OpportunitiesCount = await _context.TrainingOpportunities.CountAsync(); // all TrainingOpportunities
            bool participates = await HasActiveTrainingPlacement(student.StudentID);
            if (participates) statusTrain = TrainingStatusEnum.Active;
            else statusTrain = TrainingStatusEnum.Inactive;
            return Page();
        }


    }
}
