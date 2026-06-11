using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories
{
    public class TrainingOpportunityRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrainingOpportunityRepository(
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Queryable

        public IQueryable<TrainingOpportunity>
            GetAllQueryable()
        {
            return _dbContext.TrainingOpportunities
                .Include(x => x.TrainingInstitution)
                .Include(x => x.TrainingTerm)
                .Include(x => x.Request)
                .AsNoTracking();
        }

        public IQueryable<TrainingOpportunity>
            GetInstitutionOpportunitiesQueryable(
                int institutionId)
        {
            return _dbContext.TrainingOpportunities
                .Include(x => x.TrainingInstitution)
                .Include(x => x.TrainingTerm)
                .Include(x => x.Request)
                .Where(x =>
                    x.InstitutionID == institutionId)
                .AsNoTracking();
        }

        #endregion

        #region Get

        public async Task<List<TrainingOpportunity>>
            GetAllAsync()
        {
            return await GetAllQueryable()
                .ToListAsync();
        }

        public async Task<TrainingOpportunity?>
            GetByIdAsync(int opportunityId)
        {
            return await _dbContext
                .TrainingOpportunities
                .Include(x => x.TrainingInstitution)
                .Include(x => x.TrainingTerm)
                .FirstOrDefaultAsync(x =>
                    x.OpportunityID == opportunityId);
        }

        public async Task<TrainingOpportunity?>
            GetByIdModifyAsync(int opportunityId)
        {
            return await _dbContext
                .TrainingOpportunities
                .FirstOrDefaultAsync(x =>
                    x.OpportunityID == opportunityId);
        }

        public async Task<TrainingOpportunity?>
            GetDetailsAsync(int opportunityId)
        {
            return await _dbContext
                .TrainingOpportunities

                .Include(x => x.TrainingInstitution)

                .Include(x => x.TrainingTerm)

                .Include(x => x.Request)

                .Include(x => x.OpportunitySpecialties)
                    .ThenInclude(x => x.Specialty)

                .Include(x => x.OpportunitySkills)
                    .ThenInclude(x => x.Skill)

                .Include(x => x.TrainingPlacement)

                .AsNoTracking()

                .FirstOrDefaultAsync(x => x.OpportunityID == opportunityId);
        }

        #endregion

        #region CRUD

        public async Task AddAsync(
            TrainingOpportunity opportunity)
        {
            await _dbContext
                .TrainingOpportunities
                .AddAsync(opportunity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(
            TrainingOpportunity opportunity)
        {
            _dbContext
                .TrainingOpportunities
                .Update(opportunity);

            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Delete

        public async Task<bool>
            DeleteSoftAsync(int opportunityId)
        {
            var opportunity =
                await GetByIdModifyAsync(opportunityId);

            if (opportunity == null)
                return false;

            opportunity.Status =
                TrainingOpportunity.Opportunity.Cancelled;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task FullDeleteAsync(
            int opportunityId)
        {
            var opportunity =
                await GetByIdModifyAsync(opportunityId);

            if (opportunity == null)
            {
                throw new Exception(
                    $"Opportunity {opportunityId} Not Found");
            }

            _dbContext
                .TrainingOpportunities
                .Remove(opportunity);

            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Status

        public async Task<bool>
            ToggleStatusAsync(int opportunityId)
        {
            var opportunity =
                await GetByIdModifyAsync(opportunityId);

            if (opportunity == null)
                return false;

            opportunity.Status =
                opportunity.Status ==
                TrainingOpportunity.Opportunity.Open

                ? TrainingOpportunity.Opportunity.Closed

                : TrainingOpportunity.Opportunity.Open;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Search

        public async Task<IEnumerable<TrainingOpportunity>>
            SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<TrainingOpportunity>();

            return await _dbContext
                .TrainingOpportunities
                .Where(x =>
                    EF.Functions.Like(
                        x.Title,
                        $"%{query}%"))

                .AsNoTracking()
                .ToListAsync();
        }

        #endregion

        #region Statistics

        public async Task<int>
            GetTotalCountAsync()
        {
            return await _dbContext
                .TrainingOpportunities
                .CountAsync();
        }

        public async Task<int>
            GetOpenCountAsync()
        {
            return await _dbContext
                .TrainingOpportunities
                .CountAsync(x =>
                    x.Status ==
                    TrainingOpportunity.Opportunity.Open);
        }

        public async Task<int>
            GetClosedCountAsync()
        {
            return await _dbContext
                .TrainingOpportunities
                .CountAsync(x =>
                    x.Status ==
                    TrainingOpportunity.Opportunity.Closed);
        }

        public async Task<int>
            GetCancelledCountAsync()
        {
            return await _dbContext
                .TrainingOpportunities
                .CountAsync(x =>
                    x.Status ==
                    TrainingOpportunity.Opportunity.Cancelled);
        }

        #endregion

        #region Exists

        public async Task<bool>
            ExistsAsync(int opportunityId)
        {
            return await _dbContext
                .TrainingOpportunities
                .AnyAsync(x =>
                    x.OpportunityID == opportunityId);
        }

        #endregion
    }
}