using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories
{
    public class TrainingPlacementRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainingPlacementRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<TrainingPlacement?> GetPlacementDetailsAsync(int placementId)
        {
            return await _context.TrainingPlacements

                .Include(x => x.Student)
                    .ThenInclude(x => x.User)

                .Include(x => x.Student)
                    .ThenInclude(x => x.Specialty)

                .Include(x => x.TrainingOpportunity)

                .Include(x => x.TrainingInstitution)

                .Include(x => x.UniversitySupervisor)

                .Include(x => x.InstitutionSupervisor)

                .Include(x => x.StudentReports)

                .Include(x => x.StudentEvaluations)

                .Include(x => x.AttendanceRecords)

                .AsNoTracking()

                .FirstOrDefaultAsync(x => x.PlacementID == placementId);
        }
    }
}
