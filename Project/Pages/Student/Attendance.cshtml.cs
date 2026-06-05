using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class AttendanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 8;

        public AttendanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Display data ────────────────────────────────────────────────────
        public List<AttendanceRowViewModel> Rows       { get; set; } = new();
        public SelectList                  TermList    { get; set; } = new SelectList(Enumerable.Empty<object>());
        public int                         TotalCount  { get; set; }
        public int                         TotalPages  { get; set; }

        // ── Query params (bound from GET query string) ───────────────────────
        [BindProperty(SupportsGet = true)] public string? SearchQuery   { get; set; }
        [BindProperty(SupportsGet = true)] public string? StatusFilter  { get; set; }
        [BindProperty(SupportsGet = true)] public int?    TermFilter    { get; set; }
        [BindProperty(SupportsGet = true)] public int     CurrentPage   { get; set; } = 1;

        // ── Status options for the dropdown ─────────────────────────────────
        public List<SelectListItem> StatusOptions { get; } = new()
        {
            new SelectListItem("جميع الحالات", ""),
            new SelectListItem("حاضر",         "Present"),
            new SelectListItem("متأخر",         "Late"),
            new SelectListItem("غائب",          "Absent"),
            new SelectListItem("معذر",          "Excused"),
        };

        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login");

            // Resolve StudentId for the logged-in user
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserID == userId);

            if (student is null)
                return Page();   // No student record — Rows stays empty, view shows empty state

            // ── Active placements for the filter dropdown ──────────────────
            var placementTerms = await _context.TrainingPlacements
                .AsNoTracking()
                .Where(p => p.StudentID == student.StudentID)
                .Include(p => p.TrainingTerm)
                .Select(p => p.TrainingTerm)
                .Where(t => t != null)
                .Distinct()
                .ToListAsync();

            TermList = new SelectList(placementTerms, "TermId", "Name", TermFilter);

            // ── Base query ─────────────────────────────────────────────────
            var query = _context.AttendanceRecords
                .AsNoTracking()
                .Where(a => a.TrainingPlacement != null &&
                            a.TrainingPlacement.StudentID == student.StudentID)
                .Include(a => a.TrainingPlacement)
                    .ThenInclude(p => p!.TrainingOpportunity)
                .Include(a => a.InstitutionSupervisor)
                .AsQueryable();

            // ── Filters ────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.Trim().ToLower();
                query = query.Where(a =>
                    (a.AttendanceDate.ToString().Contains(q)) ||
                    (a.TrainingPlacement != null &&
                     a.TrainingPlacement.TrainingOpportunity != null &&
                     a.TrainingPlacement.TrainingOpportunity.Title.ToLower().Contains(q)));
            }

            //if (!string.IsNullOrWhiteSpace(StatusFilter))
            //    query = query.Where(a => a.Status == StatusFilter);

            if (TermFilter.HasValue)
                query = query.Where(a =>
                    a.TrainingPlacement != null &&
                    a.TrainingPlacement.TermID == TermFilter.Value);

            // ── Pagination ─────────────────────────────────────────────────
            TotalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            CurrentPage = Math.Clamp(CurrentPage, 1, Math.Max(1, TotalPages));

            var records = await query
                .OrderByDescending(a => a.AttendanceDate)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // ── Map to view model ──────────────────────────────────────────
            Rows = records.Select((a, idx) => new AttendanceRowViewModel
            {
                RowNumber        = (CurrentPage - 1) * PageSize + idx + 1,
                AttendanceDate   = a.AttendanceDate,
                CheckInTime      = a.CheckInTime,
                CheckOutTime     = a.CheckOutTime,
                //Status           = a.Status ?? "—",
                TrainingTitle    = a.TrainingPlacement?.TrainingOpportunity?.Title ?? "—",
                RecordedBy       = a.InstitutionSupervisor?.FullName ?? "—",
                Notes            = a.Notes,
            }).ToList();

            return Page();
        }

        // ── View model ───────────────────────────────────────────────────────
        public class AttendanceRowViewModel
        {
            public int       RowNumber      { get; set; }
            public DateTime  AttendanceDate { get; set; }
            public TimeSpan? CheckInTime    { get; set; }
            public TimeSpan? CheckOutTime   { get; set; }
            public string    Status         { get; set; } = string.Empty;
            public string    TrainingTitle  { get; set; } = string.Empty;
            public string    RecordedBy     { get; set; } = string.Empty;
            public string?   Notes          { get; set; }

            // Helpers used in Razor
            public string StatusBadgeCss => Status switch
            {
                "Present" => "badge-present",
                "Late"    => "badge-late",
                "Absent"  => "badge-absent",
                "Excused" => "badge-excused",
                _         => "badge-secondary"
            };

            public string StatusLabel => Status switch
            {
                "Present" => "حاضر",
                "Late"    => "متأخر",
                "Absent"  => "غائب",
                "Excused" => "معذر",
                _         => Status
            };

            public string CheckInDisplay  => CheckInTime.HasValue
                ? DateTime.Today.Add(CheckInTime.Value).ToString("hh:mm tt") : "—";

            public string CheckOutDisplay => CheckOutTime.HasValue
                ? DateTime.Today.Add(CheckOutTime.Value).ToString("hh:mm tt") : "—";

            public string DateDisplay =>
                AttendanceDate.ToString("yyyy-MM-dd") + "\n" +
                AttendanceDate.ToString("dddd", new System.Globalization.CultureInfo("ar-SA"));
        }
    }
}
