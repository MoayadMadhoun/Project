using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Project.Data;
using Project.Models;

namespace Project.Repository
{
    //Training Institution Repository
    public class TrainingInstitutionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TrainingInstitutionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // This is Queryable [Use In Pagination]
        public IQueryable<TrainingInstitution> GetAllQueryable() => _dbContext
            .TrainingInstitutions.Include(ti => ti.TrainingOpportunities)
            .AsNoTracking();
            


        // CRUD Methode ....

        public async Task<List<TrainingInstitution>> GetAllAsync() =>await _dbContext
            .TrainingInstitutions
            .AsNoTracking()
            .ToListAsync();

        
        public async Task<List<TrainingInstitution>> GetAllAsyncWithOpportunity()=> await _dbContext
            .TrainingInstitutions
            .Include(ti=>ti.TrainingOpportunities)
            .AsNoTracking()
            .ToListAsync();

        public async Task<TrainingInstitution?> GetByIdAsync(int? InstituationID)
        {
            return await _dbContext
                .TrainingInstitutions
                .Include(ti => ti.TrainingOpportunities)
                .FirstOrDefaultAsync(ti => ti.InstituationID == InstituationID);
        }
          
        //To Performance in Soft Delete { Take The University Without Include}
        public async Task<TrainingInstitution?> GetByIdModifyAsync(int InstituationID)
        {
            return await _dbContext
                .TrainingInstitutions
                .FirstOrDefaultAsync(ti => ti.InstituationID == InstituationID);
        }



        public async Task<Enum?> GetTypeInstitution(int InstituationID)
        {
            return await _dbContext
                .TrainingInstitutions
                .Where(ti => ti.InstituationID == InstituationID)
                .Select(ti => ti.InstitutionType)
                .FirstOrDefaultAsync();
        }


        public async Task<AspNetUser?> GetInstitutionOfficerAsync(int InstituationID)
        {

            return await _dbContext.TrainingInstitutions
                .Where(ti => ti.InstituationID == InstituationID)
                .Select
                (i => i.TrainingOpportunities
                          .Where(O => O.InstitutionOfficer != null)
                          .Select(O => O.InstitutionOfficer)
                          .FirstOrDefault()
                )
                .FirstOrDefaultAsync();
        }


        public async Task AddAsync(TrainingInstitution trainingInstitution)
        {
            trainingInstitution.IsActive = true;

            await _dbContext.TrainingInstitutions.AddAsync(trainingInstitution);

            await _dbContext.SaveChangesAsync();

        }

        public async Task UpdateAsync(TrainingInstitution trainingInstitution)
        {
            _dbContext.TrainingInstitutions.Update(trainingInstitution);

            await _dbContext.SaveChangesAsync();

        }

        // Soft Delete { I will Keep The Institution But Make It Not Active }
        public async Task<bool> DeleteSoftAsync(int InstituationID)
        {
            var instituation = await GetByIdModifyAsync(InstituationID);



            if (instituation is null)
                return false;

            if (!instituation.IsActive)
                return false;


            instituation.IsActive = false;

            await _dbContext.SaveChangesAsync();
            return true;



        }

        // Full Delete { I Will Delete The institution From The DataBase }
        public async Task FullDeleteInstitution(int InstituationID)
        {
            var instituation = await GetByIdModifyAsync(InstituationID);

            if (instituation is not null)
            {

                _dbContext.TrainingInstitutions.Remove(instituation);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Unable To Find the Instituation {InstituationID}");
            }
        }


         public async Task<bool> ExistsAny(int InstituationID)
         {
              return await _dbContext
                .TrainingInstitutions
                .AnyAsync(u => u.InstituationID == InstituationID);
         }


        // Filtring Methode .....
        public async Task<IEnumerable<TrainingInstitution?>> GetBySearchAsync(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<TrainingInstitution?>();

            return await _dbContext.TrainingInstitutions
                .Where(u => EF.Functions.Like(u.Name, $"%{query}%"))
                .AsNoTracking()
                .ToListAsync();

        }


        public async Task<IEnumerable<TrainingInstitution>> GetByStatusAsync(bool isActive)
        {
            return await _dbContext.TrainingInstitutions
                .Where(u => u.IsActive == isActive)
                .ToListAsync();

        }

        public async Task<int> GetTotalCountAsync()
        {
            // All TrainingInstitution In The Database 
            return await _dbContext
                .TrainingInstitutions
                .CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {

            return await _dbContext
                .TrainingInstitutions
                .CountAsync(u => u.IsActive);
        }

        public async Task<int> GetInActiveCountAsync()
        {
            return await _dbContext
                .TrainingInstitutions
                .CountAsync(u => !u.IsActive);
        }



        // Toggle Statuse 
        public async Task<bool> ToggleStatusAsync(int InstituationID)
        {
            var instituation = await GetByIdModifyAsync(InstituationID);

            if (instituation is null)
                return false;

            instituation.IsActive = !instituation.IsActive;
            await _dbContext.SaveChangesAsync();
            return true;


        }
        //Get Applications for Institution
        public IQueryable<TrainingApplication> GetApplicationsForInstitution(int? InstituationID)
        {
            return _dbContext.TrainingApplications
                .Include(a => a.Student)
                .ThenInclude(s => s.Department)
                .ThenInclude(d => d.University)
                .Include(a => a.TrainingOpportunity)
                .Where(a => a.TrainingOpportunity.InstitutionID == InstituationID)
                .AsNoTracking().AsQueryable();
        }
        public IQueryable<TrainingOpportunity> GetTrainingOpportunitiesQueryable(int? InstituationID)
        {
            return _dbContext.TrainingOpportunities
                .Include(tr=>tr.OpportunitySpecialties)
                .ThenInclude(s=>s.Specialty)
                .Where(t=>t.InstitutionID== InstituationID)
                .AsNoTracking().AsQueryable();
        }
        public IQueryable<AspNetUser> GetSupervisorsForInstitution(int? InstitutionId)
        {
            return _dbContext.Users
                .Include(u => u.RoleScope)
                .ThenInclude(rs => rs.Department)
                .Where(u => u.RoleScope.Role.Name == "InstitutionSupervisor" && u.RoleScope.InstitutionID == InstitutionId)
                .AsNoTracking().AsQueryable();
        }
        public IQueryable<TrainingOpportunity> GetCurrentTrainingsForInstitution(int institutionId)
        {
            return _dbContext.TrainingOpportunities
                .Include(to => to.TrainingPlacement)
                .Where(to => to.InstitutionID == institutionId && to.TrainingPlacement.Any(tp=>tp.Status== TrainingPlacement.PlacementStatus.InProgress))
                .AsNoTracking().AsQueryable();
        }
        public IQueryable<AttendanceRecord> GetAttendanceForInstitution(int institutionId)
        {
            return _dbContext.AttendanceRecords
                .Include(to => to.TrainingPlacement)
                .ThenInclude( p => p.Student)
                .Where(to => to.TrainingPlacement.InstitutionID == institutionId)
                .AsNoTracking().AsQueryable();
        }
    }
}

