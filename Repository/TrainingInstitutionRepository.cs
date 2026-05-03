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
            .AsNoTracking()
            .AsQueryable();


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

        public async Task<TrainingInstitution?> GetByIdAsync(int InstituationID)
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



        public async Task<string?> GetTypeInstitution(int InstituationID)
        {
            return await _dbContext
                .TrainingInstitutions
                .Where(ti => ti.InstituationID == InstituationID)
                .Select(ti => ti.Type)
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

        // Soft Delete { I will Keep The University But Make It Not Active }
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

        // Full Delete { I Will Delete The University From The DataBase }
        public async Task FullDeleteUniversity(int InstituationID)
        {
            var instituation = await GetByIdAsync(InstituationID);

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
            return await _dbContext.TrainingInstitutions
                .Where(u => EF.Functions.Like(u.Name, $"%{query}%"))
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
            var instituation = await GetByIdAsync(InstituationID);

            if (instituation is null)
                return false;

            instituation.IsActive = !instituation.IsActive;
            await _dbContext.SaveChangesAsync();
            return true;


        }




       


        


    }


}

