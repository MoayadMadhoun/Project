using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories
{
    public class DepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DepartmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IQueryable<Department> GetAllQueryable() => _dbContext
           .Departments
           .Include(d => d.Students)
           .Include(d=>d.Specialties)
           .AsNoTracking();


        public async Task<List<Department>> GetAllAsync() => await _dbContext
           .Departments           
           .AsNoTracking()
           .ToListAsync();

        public async Task<List<Department>> GetDepWithStudent() => await _dbContext
           .Departments
           .Include(d => d.Students)
           .AsNoTracking()
           .ToListAsync();


        public async Task<Department?> GetByIdAsync(int DepartmentID)
        {
            return await _dbContext
            .Departments
            .Include(d => d.Students)
            .Include(d => d.Specialties)
            .Include(d=>d.University)
            .ThenInclude(u=>u.TrainingOpportunityRequests)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DepartmentID == DepartmentID);
        }


        public async Task<Department?> GetByIdModifyAsync(int DepartmentID) => await _dbContext
           .Departments
           .FirstOrDefaultAsync(d => d.DepartmentID == DepartmentID);


        public async Task AddAsync(Department department )
        {

             _dbContext.Departments.Add(department);

            await _dbContext.SaveChangesAsync();

        }


        public async Task UpdateAsync(Department department)
        {
            _dbContext.Departments.Update(department);

            await _dbContext.SaveChangesAsync();

        }


        // Soft Delete { I will Keep The Department But Make It Not Active }
        public async Task<bool> DeleteSoftAsync(int DepartmentID)
        {
            var department = await GetByIdModifyAsync(DepartmentID);



            if (department is null)
                return false;

            if (!department.IsActive)
                return false;


            department.IsActive = false;

            await _dbContext.SaveChangesAsync();
            return true;



        }


        // Full Delete { I Will Delete The Department From The DataBase }
        public async Task FullDeleteUniversity(int DepartmentID)
        {
            var department = await GetByIdModifyAsync(DepartmentID);

            if (department is not null)
            {

                _dbContext.Departments.Remove(department);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Unable To Find the University {DepartmentID}");
            }

        }

        public async Task<bool> ExistsAny(int DepartmentID)
        {
            return await _dbContext
                .Departments
                .AnyAsync(d => d.DepartmentID == DepartmentID);
        }


        // Filtring Methode .....
        public async Task<IEnumerable<Department?>> GetBySearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Department?>();

            return await _dbContext.Departments
            .Where(d => EF.Functions.Like(d.Name, $"%{query}%"))
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetByStatusAsync(bool isActive)
        {

            return await _dbContext.Departments
                .Where(d => d.IsActive == isActive)
                .ToListAsync();

        }


        // Count 
        public async Task<int> GetTotalCountAsync()
        {
            // All Department In The Database 
            return await _dbContext.Departments.CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {

            return await _dbContext.Departments.CountAsync(d => d.IsActive);
        }

        public async Task<int> GetInActiveCountAsync()
        {
            return await _dbContext.Departments.CountAsync(d => !d.IsActive);
        }


        // Toggle Statuse 
        public async Task<bool> ToggleStatusAsync(int DepartmentID)
        {
            var department = await GetByIdModifyAsync(DepartmentID);

            if (department is null)
                return false;

            department.IsActive = !department.IsActive;
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<List<SelectListItem>> GetUniversityDepartmentsSelectListAsync(int universityId)
        {
            return await _dbContext.Departments
                .Where(d => d.UniversityID == universityId)
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentID.ToString(),
                    Text = d.Name
                })
                .ToListAsync();
        }



        























    }











}
