using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using Project.Repostory;

namespace Project.Pages.University
{
    [Authorize(Roles = "UniversityTrainingAdmin, DepartmentHead ,UniversitySupervisor")]
    public class AttendanceModel : PageModel
    {
        private readonly UniversityRepository _universityRepo;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly StudentsRepository _studentRepo;

        public AttendanceModel(UniversityRepository universityRepo, UserManager<AspNetUser> userManager, ApplicationDbContext dbContext, StudentsRepository studentRepo)
        {
            _universityRepo = universityRepo;
            _userManager = userManager;
            _dbContext = dbContext;
            _studentRepo = studentRepo;
        }
        [BindProperty(SupportsGet = true)]
        public int? StudentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        public Models.University CurrentUniversity { get; set; }
        public SelectList Institutions {  get; set; }
        [BindProperty(SupportsGet = true)]
        public int SelectedInstitutionId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SelectedAttendanceStatusId { get; set; }
        public PaginatedList<Models.AttendanceRecord> AttendanceRecords { get; set; } = new PaginatedList<Models.AttendanceRecord>(new List<Models.AttendanceRecord>(), 0, 1, 10);
        public Models.Student? SelectedStudent { get; set; }

        public async Task<IActionResult> OnGet()
        {

            var user = await _userManager.GetUserAsync(User);
            AspNetRoleScope? scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

            if (scope == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (scope.UniversityID == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            int universityId = (int)scope.UniversityID;

            CurrentUniversity = await _universityRepo.GetByIdAsync(universityId);
            if (CurrentUniversity == null)
            {
                return RedirectToPage("/Index");
            }
            var query = _dbContext.AttendanceRecords
             .Include(at => at.TrainingPlacement)
                 .ThenInclude(tp => tp.Student)
             .Include(at => at.TrainingPlacement)
                 .ThenInclude(tp => tp.TrainingInstitution)
             .Include(at => at.TrainingPlacement)
                 .ThenInclude(tp => tp.TrainingOpportunity)
             .Where(at =>
                 at.TrainingPlacement.Student.UniversityID == universityId)
             .AsQueryable();

            if (StudentId.HasValue)
            {
                query = query.Where(a =>
                    a.TrainingPlacement.StudentID ==
                    StudentId.Value);
            }
            if (StudentId.HasValue)
            {
                SelectedStudent = await _dbContext.Students
                    .FirstOrDefaultAsync(s =>
                        s.StudentID == StudentId.Value);
            }
            if (User.IsInRole("DepartmentHead"))
            {
                query = query.Where(at=>at.TrainingPlacement.Student.DepartmentID == scope.DepartmentID);
            }
            //filtring
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(a => a.TrainingPlacement.Student.Name.Contains(SearchTerm));
            }

            if (SelectedAttendanceStatusId > 0)
            {
                var AttendanceStatus = SelectedAttendanceStatusId switch
                {
                    1 => AttendanceRecord.AttendanceStatus.Present,
                    2 => AttendanceRecord.AttendanceStatus.Late,
                    3 => AttendanceRecord.AttendanceStatus.Absent,
                    4 => AttendanceRecord.AttendanceStatus.Excused,
                    _ => AttendanceRecord.AttendanceStatus.Present,

                };
                query = query.Where(a => a.Status == AttendanceStatus);
            }
            var institutionList = AttendanceRecords.Select(ar => ar.TrainingPlacement.TrainingInstitution).Distinct().ToList();
            Institutions = new SelectList(institutionList, "InstituationID", "Name");
            if (SelectedInstitutionId > 0) {
                query.Where(ar => ar.TrainingPlacement.TrainingInstitution.InstituationID == SelectedInstitutionId);
            }
            //sorting
            query = SortOrder switch
            {
                "Name" => query.OrderBy(a => a.TrainingPlacement.Student.Name),
                "Name_desc" => query.OrderByDescending(a => a.TrainingPlacement.Student.Name),
               
                _ => query.OrderBy(p => p.AttendanceID),
            };
            //pagination
            AttendanceRecords = await PaginatedList<AttendanceRecord>.CreateAsync(query, PageSize, PageIndex);
            return Page();
        }
    }
}
