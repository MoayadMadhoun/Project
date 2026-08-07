using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "DepartmentHead, UniversityTrainingAdmin")]
    public class DeleteModel : PageModel
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly UniversityRepository _universityRepo;
        private readonly TrainingInstitutionRepository _institutionRepository;


        public DeleteModel(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, UniversityRepository universityRepo, TrainingInstitutionRepository institutionRepository)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _universityRepo = universityRepo;
            _institutionRepository = institutionRepository;

        }
        public TrainingOpportunityRequest? TrainingOpportunityRequest { get; set; } = new TrainingOpportunityRequest();


        public IActionResult OnGet([FromRoute] int RequestId)
        {
            TrainingOpportunityRequest = _dbContext.TrainingOpportunityRequests.Include(to=>to.TrainingTerm).Where(to => to.RequestID == RequestId).FirstOrDefault();
            if(TrainingOpportunityRequest == null)
            {
                return NotFound();
            }
            return Page();

        }
        public async Task<IActionResult> OnPost([FromRoute] int RequestId)
        {
            TrainingOpportunityRequest = _dbContext.TrainingOpportunityRequests.Include(to => to.TrainingTerm).Where(to => to.RequestID == RequestId).FirstOrDefault();

            if (TrainingOpportunityRequest == null)
            {
                return NotFound();
            }
             _dbContext.TrainingOpportunityRequests.Remove(TrainingOpportunityRequest);
            await _dbContext.SaveChangesAsync();
            var requestInstitutions= _dbContext.OpportunityRequestInstitutions.Where(rt => rt.RequestID == TrainingOpportunityRequest.RequestID);
            if (requestInstitutions.Count() > 0)
            {
                foreach (var rt in requestInstitutions)
                {
                    _dbContext.OpportunityRequestInstitutions.Remove(rt);
                }
            }
            await _dbContext.SaveChangesAsync();
              return RedirectToPage("/University/AvailabelOpportunities");

        }
    }
}
