using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repository;
using Project.Repostory;

namespace Project.Pages.University.TrainingRequests
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly UniversityRepository _universityRepo;
        private readonly TrainingInstitutionRepository _institutionRepository;


        public DetailsModel(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, UniversityRepository universityRepo, TrainingInstitutionRepository institutionRepository)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _universityRepo = universityRepo;
            _institutionRepository = institutionRepository;

        }
        public TrainingOpportunityRequest? TrainingOpportunityRequest { get; set; } = new TrainingOpportunityRequest();
        public IActionResult OnGet([FromRoute] int RequestId)
        {

            TrainingOpportunityRequest = _dbContext.TrainingOpportunityRequests
                .Include(to => to.TrainingTerm)
                 .Include(to => to.University)
                . Include(to => to.UniversityAdmin)
                . Include(to => to.Department)
                 .Include(to => to.DepartmentHead)
                .Where(to => to.RequestID == RequestId).FirstOrDefault();
            if (TrainingOpportunityRequest == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
