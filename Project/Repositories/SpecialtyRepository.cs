using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories
{
    public class SpecialtyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SpecialtyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // This is Queryable [Use In Pagination]
        public IQueryable<Specialty> GetAllQueryable() => _dbContext
            .Specialties
            .Include(s => s.Students)
            .Include(s => s.OpportunitySpecialties)
            .Include(s => s.RequestSpecialties)
            .AsNoTracking();



        // CRUD Methode ....

        public async Task<List<Specialty>> GetAllAsync() => await _dbContext
            .Specialties
            .AsNoTracking()
            .ToListAsync();


        public async Task<Specialty?> GetByIdAsync(int SpecialtyID)
        {
            return await _dbContext
            .Specialties
            .Include(s=>s.Students)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SpecialtyID == SpecialtyID);
        }

        //To Performance in Soft Delete { Take The specialty Without Include}
        public async Task<Specialty?> GetByIdModifyAsync(int SpecialtyID) => await _dbContext
           .Specialties
           .FirstOrDefaultAsync(s => s.SpecialtyID == SpecialtyID);


        public async Task AddAsync(Specialty specialty)
        {
            specialty.IsActive = true;

            await _dbContext.Specialties.AddAsync(specialty);

            await _dbContext.SaveChangesAsync();

        }


        public async Task UpdateAsync(Specialty specialty)
        {
            _dbContext.Specialties.Update(specialty);

            await _dbContext.SaveChangesAsync();

        }

        // Soft Delete { I will Keep The specialty But Make It Not Active }
        public async Task<bool> DeleteSoftAsync(int SpecialtyID)
        {
            var specialty = await GetByIdModifyAsync(SpecialtyID);



            if (specialty is null)
                return false;

            if (!specialty.IsActive)
                return false;


            specialty.IsActive = false;

            await _dbContext.SaveChangesAsync();
            return true;



        }


        // Full Delete { I Will Delete The specialty From The DataBase }
        public async Task FullDeleteUniversity(int SpecialtyID)
        {
            var specialty = await GetByIdModifyAsync(SpecialtyID);

            if (specialty is not null)
            {

                _dbContext.Specialties.Remove(specialty);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Unable To Find the University {SpecialtyID}");
            }

        }


        public async Task<bool> ExistsAny(int SpecialtyID)
        {
            return await _dbContext.Specialties.AnyAsync(s => s.SpecialtyID == SpecialtyID);
        }

        // Filtring Methode .....
        public async Task<IEnumerable<Specialty?>> GetBySearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Specialty?>();



            return await _dbContext.Specialties
            .Where(u => EF.Functions.Like(u.Name, $"%{query}%"))
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<IEnumerable<Specialty>> GetByStatusAsync(bool isActive)
        {

            return await _dbContext.Specialties
                .Where(s => s.IsActive == isActive)
                .ToListAsync();

        }


        // Count 
        public async Task<int> GetTotalCountAsync()
        {
            // All University In The Database 
            return await _dbContext.Specialties.CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {

            return await _dbContext.Specialties.CountAsync(s => s.IsActive);
        }

        public async Task<int> GetInActiveCountAsync()
        {
            return await _dbContext.Specialties.CountAsync(s => !s.IsActive);
        }


        // Toggle Statuse 
        public async Task<bool> ToggleStatusAsync(int SpecialtyID)
        {
            var specialty = await GetByIdModifyAsync(SpecialtyID);

            if (specialty is null)
                return false;

            specialty.IsActive = !specialty.IsActive;
            await _dbContext.SaveChangesAsync();
            return true;
        }




        public async Task<SelectList> CreateSpecialtySelectList()
        {
            return new SelectList(await GetByStatusAsync(true), "SpecialtyID", "Name");

        }


        public IQueryable<Specialty> GetUniversitySpecialtiesQueryable(int universityId)
        {
            return  _dbContext.Specialties
                .Include(s => s.Department)
                .Include(s => s.Students)
                .Where(s => s.Department.UniversityID == universityId)
                .AsNoTracking();
        }

        public async Task<Specialty?> GetDetailsAsync(int specialtyId)
        {
            return await _dbContext.Specialties
                .Include(s => s.Department)
                .Include(s => s.Students)
                .Include(s => s.OpportunitySpecialties)
                .Include(s => s.RequestSpecialties)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SpecialtyID == specialtyId);
        }
















































    }










































}
