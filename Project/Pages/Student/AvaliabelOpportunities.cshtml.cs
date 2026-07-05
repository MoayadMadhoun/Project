using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using Project.Data;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class AvaliabelOpportunitiesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentsRepository _studentRepo;
        private readonly SpecialtyRepository _specialtyRepository;

        [BindProperty(SupportsGet = true)]
        public string? search { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? specialtyId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? location { get; set; }

        [BindProperty(SupportsGet = true)]
        public TrainingOpportunity.Opportunity? status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public SelectList? Location { get; set; }
        public SelectList? Specialty { get; set; }
        public HashSet<int> AppliedIds { get; set; } = new();
        public IQueryable<TrainingApplication> applications { get; set; }
       public TrainingPlacement? CurrentTraining { get; set; }
        public PaginatedList<TrainingOpportunity>? Opportunities { get; set; }
        public int CurrentCount => Opportunities?.Count() ?? 0;
        public int TotalCount { get; set; }
        

        public AvaliabelOpportunitiesModel(ApplicationDbContext context, StudentsRepository studentRepo, SpecialtyRepository SpecialtyRepository)
        {
            _context = context;
            _studentRepo = studentRepo;
            _specialtyRepository = SpecialtyRepository;
        }
        public string GetDateOrMonth(TrainingOpportunity trainOpportunity)
        {
            int days = (trainOpportunity.EndDate - trainOpportunity.StartDate).Days;

            if (days < 30)
            {
                return $"{days} يوم";
            }

            int months = days / 30;

            if (months == 1) return "شهر";
            if (months == 2) return "شهرين";
            return $"{months} اشهر ";
        }
        public async Task<IActionResult> OnGet()
        {
            var city = await _context.TrainingOpportunities.Where(p => p.Location != null).Select(p => p.Location).Distinct().ToListAsync();
            var state = await _context.TrainingOpportunities.Select(p => p.Status).Distinct().ToListAsync();
            Location = new SelectList(city);
            Specialty = await _specialtyRepository.CreateSpecialtySelectList();
            var Opportunity = _context.TrainingOpportunities.Include(t=>t.OpportunitySpecialties).ThenInclude(s => s.Specialty).Include(t => t.TrainingInstitution).AsNoTracking();
          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var student = await _studentRepo.GetStudentByUserId(userId);

            if (student == null) return NotFound();

             applications = _studentRepo.GetAllApplicationByStudentId(student.StudentID);

            AppliedIds = applications.Select(a => a.OpportunityID).ToHashSet();

             CurrentTraining = await  _context.TrainingPlacements.FirstOrDefaultAsync(x=>x.StudentID == student.StudentID);
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                Opportunity = Opportunity.Where(
                O => EF.Functions.Like(O.Title, $"%{search}%") ||
                EF.Functions.Like(O.OpportunitySpecialties.Select(s => s.Specialty.Name).FirstOrDefault(), $"%{search}%") ||
                EF.Functions.Like(O.TrainingInstitution.Name, $"%{search}%"));
                
            }
            if (specialtyId.HasValue)
            {
                Opportunity = Opportunity.Where(O =>  O.OpportunitySpecialties.Any(s => s.SpecialtyID == specialtyId));
            }
            if (status.HasValue)
            {
 
                Opportunity = Opportunity.Where(O => O.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(location))
            {
                Opportunity = Opportunity.Where(O => O.Location != null && O.Location.Equals(location));
            }
            
            TotalCount = await Opportunity.CountAsync();

            Opportunities = await PaginatedList<TrainingOpportunity>.CreateAsync(Opportunity, PageSize, PageIndex);

            return Page();
        }
    }
}
