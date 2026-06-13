using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.EntityFrameworkCore;

using Project.Data;

using Project.Extensions;

using Project.Models;

using Project.Pages.Institution.ViewModels;

using Project.Repository;



namespace Project.Pages.Institution

{

    [Authorize(Roles = "InstitutionTrainingOfficer,InstitutionSupervisor")]

    public class CurrentTrainingModel : PageModel

    {

        private readonly ApplicationDbContext _dbContext;

        private readonly TrainingInstitutionRepository _institutionRepository;

        private readonly UserManager<AspNetUser> _userManager;



        public CurrentTrainingModel(

            ApplicationDbContext dbContext,

            TrainingInstitutionRepository institutionRepository,

            UserManager<AspNetUser> userManager)

        {

            _dbContext = dbContext;

            _institutionRepository = institutionRepository;

            _userManager = userManager;

        }



        [BindProperty(SupportsGet = true)]

        public string? SearchTerm { get; set; }



        [BindProperty(SupportsGet = true)]

        public int PageIndex { get; set; } = 1;



        [BindProperty(SupportsGet = true)]

        public int PageSize { get; set; } = 10;



        public PaginatedList<TrainingOpportunity> CurrentTrainings { get; set; } =

            new PaginatedList<TrainingOpportunity>(new List<TrainingOpportunity>(), 1, 0, 10);



        public TrainingInstitution? CurrentInstitution { get; private set; }



        public static string GetDurationText(DateTime startDate, DateTime endDate) =>

            TrainingDetailsVM.GetDurationText(startDate, endDate);



        public static int GetTraineeCount(TrainingOpportunity opportunity)
        {
            return opportunity.TrainingPlacement
                .Count(tp =>
                    tp.Status !=
                    TrainingPlacement.PlacementStatus.Cancelled);
        }


        public async Task<IActionResult> OnGetAsync()

        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)

                return RedirectToPage("/Account/Login", new { area = "Identity" });



            var scope = await _dbContext.AspNetRoleScopes

                .AsNoTracking()

                .FirstOrDefaultAsync(s => s.UserID == user.Id && s.IsActive);



            if (scope?.InstitutionID == null)

                return RedirectToPage("/Account/Login", new { area = "Identity" });



            var institutionId = scope.InstitutionID.Value;

            CurrentInstitution = await _institutionRepository.GetByIdAsync(institutionId);

            if (CurrentInstitution == null)

                return RedirectToPage("/Index");



            var query  = _institutionRepository.GetTrainingsForInstitution(institutionId);


            if (!string.IsNullOrWhiteSpace(SearchTerm))

                query = query.Where(tp => tp.Title.Contains(SearchTerm));



            CurrentTrainings = await PaginatedList<TrainingOpportunity>.CreateAsync(query, PageSize, PageIndex);

            return Page();

        }


        public static string GetTrainingStatus(TrainingOpportunity opportunity)
        {
            var today = DateTime.Today;

            if (today < opportunity.StartDate)
                return "قادم";

            if (today > opportunity.EndDate)
                return "منتهي";

            return "نشط";
        }

        public static string GetTrainingStatusClass(TrainingOpportunity opportunity)
        {
            var today = DateTime.Today;

            if (today < opportunity.StartDate)
                return "bg-blue-50 text-blue-700";

            if (today > opportunity.EndDate)
                return "bg-red-50 text-red-700";

            return "bg-green-50 text-green-700";
        }

    }

}


