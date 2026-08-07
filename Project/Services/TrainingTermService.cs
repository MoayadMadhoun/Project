using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Services
{
    public class TrainingTermService
    {
        private readonly ApplicationDbContext _context;

        public TrainingTermService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CreateAsync(TrainingTerm model)
        {
            if (model.EndDate <= model.StartDate)
                return (false, "يجب أن يكون تاريخ النهاية بعد تاريخ البداية.");

            // توليد السنة الأكاديمية تلقائياً إذا كانت فارغة
            if (string.IsNullOrWhiteSpace(model.AcademicYear))
            {
                int firstYear;
                int secondYear;

                if (model.StartDate.Month >= 9)
                {
                    firstYear = model.StartDate.Year;
                    secondYear = model.StartDate.Year + 1;
                }
                else
                {
                    firstYear = model.StartDate.Year - 1;
                    secondYear = model.StartDate.Year;
                }

                model.AcademicYear = $"{firstYear}/{secondYear}";
            }

            // منع تكرار الاسم في نفس السنة الأكاديمية
            bool exists = await _context.TrainingTerms.AnyAsync(x =>
                x.Name.Trim() == model.Name.Trim() &&
                x.AcademicYear == model.AcademicYear);

            if (exists)
                return (false, "يوجد فترة تدريب بنفس الاسم في هذه السنة الأكاديمية.");

            // منع تداخل الفترات
            bool overlap = await _context.TrainingTerms.AnyAsync(x =>
                model.StartDate <= x.EndDate &&
                model.EndDate >= x.StartDate);

            if (overlap)
                return (false, "الفترة تتداخل مع فترة تدريب أخرى.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // السماح بفترة مفعلة واحدة فقط
                //if (model.IsActive)
                //{
                //    var activeTerms = await _context.TrainingTerms
                //        .Where(x => x.IsActive)
                //        .ToListAsync();

                //    foreach (var term in activeTerms)
                //    {
                //        term.IsActive = false;
                //    }
                //}

                _context.TrainingTerms.Add(model);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (true, "تم إنشاء فترة التدريب بنجاح.");
            }
            catch
            {
                await transaction.RollbackAsync();

                return (false, "حدث خطأ أثناء حفظ البيانات.");
            }
        }
    }
}