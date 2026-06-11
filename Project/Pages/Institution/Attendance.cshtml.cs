using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using Project.Repository;

namespace Project.Pages.Institution
{
    public class AttendanceModel : PageModel
    {
       
   
        private readonly ApplicationDbContext _dbContext;
        private readonly StudentsRepository _studentsRepository;
        private readonly TrainingInstitutionRepository _institutionRepository;
        private readonly UserManager<AspNetUser> _userManager;
        public AttendanceModel(ApplicationDbContext dbContext, StudentsRepository studentsRepository, TrainingInstitutionRepository institutionRepository, UserManager<AspNetUser> userManager)
        {
            _dbContext = dbContext;
            _studentsRepository = studentsRepository;
            _institutionRepository = institutionRepository;
            _userManager = userManager;
        }
        [BindProperty(SupportsGet = true)]
        public int SelectedAttendanceStatusId { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime SelectedDate { get; set;  }
        [BindProperty(SupportsGet = true)]
        public string SelectedTrainig {  get; set; }
        public SelectList Trainings { get; set; }
        public PaginatedList<AttendanceRecord>  AttendanceRecords { get; set; }
        public TrainingInstitution? CurrentInstitution { get; private set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public List<SelectListItem> StatusOptions { get; } = new()
        {
            new SelectListItem("جميع الحالات", ""),
            new SelectListItem("حاضر",         "Present"),
            new SelectListItem("متأخر",         "Late"),
            new SelectListItem("غائب",          "Absent"),
            new SelectListItem("معذر",          "Excused"),
        };
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                AspNetRoleScope? scope = _dbContext.AspNetRoleScopes.FirstOrDefault(s => s.UserID == user.Id && s.IsActive);

                if (scope == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                if (scope.InstitutionID == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                int institutionId = (int)scope.InstitutionID;

                CurrentInstitution = await _institutionRepository.GetByIdAsync(institutionId);
                if (CurrentInstitution == null)
                {
                    return RedirectToPage("/Index");
                }
                var query = _institutionRepository.GetAttendanceForInstitution(institutionId);
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(tp => tp.TrainingPlacement.Student.Name.Contains(SearchTerm));
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
                var TrainingList =  await query
                .Select(ar => ar.TrainingPlacement.TrainingOpportunity.Title)
                .Distinct()
                .ToListAsync();
                Trainings = new SelectList(TrainingList);
                if (!string.IsNullOrEmpty(SelectedTrainig))
                {
                   query = query.Where(ar => ar.TrainingPlacement.TrainingOpportunity.Title == SelectedTrainig);
                }
                if (SelectedDate != default)
                {
                    query = query.Where(ar => ar.AttendanceDate.Date == SelectedDate.Date);
                }

                //pagination
                AttendanceRecords = await PaginatedList<AttendanceRecord>.CreateAsync(query, PageSize, PageIndex);
                return Page();


            }
            catch (Exception ex)
            {
                return RedirectToPage("/Index");
            }

        }
    }
}
