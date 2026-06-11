using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Data;
using Project.Models;
namespace Project.Repostory
{
    // UNIVERSITY REPOSITORY
    public class UniversityRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UniversityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // This is Queryable [Use In Pagination]
        public IQueryable<University> GetAllQueryable() => _dbContext
            .Universities.Include(u => u.Departments)
            .AsNoTracking();



        // CRUD Methode ....

        public async Task<List<University>> GetAllAsync() => await _dbContext
            .Universities
            .Include(u => u.Departments)
            .AsNoTracking()
            .ToListAsync();


        public async Task<University?> GetByIdAsync(int UniversityID)
        {
            return await _dbContext
            .Universities
            .Include(u => u.Departments)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UniversityID == UniversityID);
        }

        //To Performance in Soft Delete { Take The University Without Include}
        public async Task<University?> GetByIdModifyAsync(int UniversityID) => await _dbContext
           .Universities
           .FirstOrDefaultAsync(u => u.UniversityID == UniversityID);


        public async Task AddAsync(University university)
        {
            university.IsActive = true;

            await _dbContext.Universities.AddAsync(university);

            await _dbContext.SaveChangesAsync();

        }


        public async Task UpdateAsync(University university)
        {
            _dbContext.Universities.Update(university);

            await _dbContext.SaveChangesAsync();

        }

        // Soft Delete { I will Keep The University But Make It Not Active }
        public async Task<bool> DeleteSoftAsync(int UniversityID)
        {
            var university = await GetByIdModifyAsync(UniversityID);



            if (university is null)
                return false;

            if (!university.IsActive)
                return false;


            university.IsActive = false;

            await _dbContext.SaveChangesAsync();
            return true;



        }


        // Full Delete { I Will Delete The University From The DataBase }
        public async Task FullDeleteUniversity(int UniversityID)
        {
            var university = await GetByIdModifyAsync(UniversityID);

            if (university is not null)
            {

                _dbContext.Universities.Remove(university);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Unable To Find the University {UniversityID}");
            }

        }


        public async Task<bool> ExistsAny(int UniversityID)
        {
            return await _dbContext.Universities.AnyAsync(u => u.UniversityID == UniversityID);
        }

        // Filtring Methode .....
        public async Task<IEnumerable<University?>> GetBySearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<University?>();



            return await _dbContext.Universities
            .Where(u => EF.Functions.Like(u.Name, $"%{query}%"))
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<IEnumerable<University>> GetByStatusAsync(bool isActive)
        {

            return await _dbContext.Universities
                .Where(u => u.IsActive == isActive)
                .ToListAsync();

        }


        // Count 
        public async Task<int> GetTotalCountAsync()
        {
            // All University In The Database 
            return await _dbContext.Universities.CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {

            return await _dbContext.Universities.CountAsync(u => u.IsActive);
        }

        public async Task<int> GetInActiveCountAsync()
        {
            return await _dbContext.Universities.CountAsync(u => !u.IsActive);
        }


        // Toggle Statuse 
        public async Task<bool> ToggleStatusAsync(int UniversityID)
        {
            var university = await GetByIdModifyAsync(UniversityID);

            if (university is null)
                return false;

            university.IsActive = !university.IsActive;
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<SelectList> CreateUniversitySelectList()
        {
            return new SelectList(await GetByStatusAsync(true), "UniversityID", "Name");

            //return new SelectList(await GetByStatusAsync(true), nameof(University.UniversityID), nameof(University.Name));
        }

        public IQueryable<TrainingApplication> GetApplicationsForUniversity(int universityId)
        {
            return _dbContext.TrainingApplications
                .Where(a => a.Student.Department.University.UniversityID == universityId)
                .Include(a => a.Student)
                .ThenInclude(s => s.Department)
                .ThenInclude(d => d.University)
                .Where(app => app.Student.Department.UniversityID== universityId)
                .Include(a => a.TrainingOpportunity)
                .ThenInclude(t=>t.TrainingInstitution)
                .AsNoTracking().AsQueryable();
        }
        //include institution
        public IQueryable<TrainingOpportunityRequest> GetOpportunityRequestForUniversity(int? UniversityId)
        {
            return _dbContext.TrainingOpportunityRequests
                .Include(r => r.Specialties)
                .ThenInclude(rs => rs.Specialty)
                .Include(r => r.OpportunityRequests)
                .ThenInclude(ti => ti.TrainingInstitution)
                .Where(or => or.UniversityID == UniversityId)
                .AsNoTracking().AsQueryable();
        }
        public IQueryable<AspNetUser> GetDepartmentHeadsForUniversity(int? UniversityId)
        {
            return _dbContext.Users
                .Include(u => u.RoleScope)
                .ThenInclude(rs => rs.Department)
                .Where(u=>u.RoleScope.Role.Name=="DepartmentHead" && u.RoleScope.UniversityID==UniversityId)
                .AsNoTracking().AsQueryable();
        }

        public async Task<AspNetUser?> GetDpartmentHeadByUserId(string UserId)
        {
            return await _dbContext.Users
                .Include(u => u.RoleScope)
                    .ThenInclude(rs => rs.Department)
                .Include(u => u.RoleScope)
                    .ThenInclude(rs => rs.University)
                .FirstOrDefaultAsync(u => u.Id == UserId);
        }
        public async Task<AspNetUser?> GetUniversitySuperVisorByUserId(string UserId)
        {
            return await _dbContext.Users
             .Include(u => u.RoleScope)
                 .ThenInclude(rs => rs.University)
             
             .FirstOrDefaultAsync(u => u.Id == UserId);
        }


    }
}