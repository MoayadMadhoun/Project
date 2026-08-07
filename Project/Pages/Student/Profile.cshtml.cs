// Pages/Student/Profile.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Pages.Student.StudentViewModels;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class ProfileModel : PageModel
    {
        private readonly StudentsRepository _studentsRepository;
        private readonly ApplicationDbContext _context;

        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const long MaxCvSizeBytes = 10 * 1024 * 1024;

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        private static readonly string[] AllowedCvExtensions = { ".pdf", ".doc", ".docx" };

        public ProfileModel(
            StudentsRepository studentsRepository,
            ApplicationDbContext context)
        {
            _studentsRepository = studentsRepository;
            _context = context;
        }

        public SelectList Specialties { get; set; } = default!;
        public SelectList Skills { get; set; } = default!;

        [BindProperty]
        public IFormFile? ProfileImage { get; set; }

        [BindProperty]
        public IFormFile? CVFile { get; set; }

        [BindProperty]
        public Project.Models.Student CurrentStudent { get; set; } = new();

        [BindProperty]
        public AddStudentSkillVM NewSkill { get; set; } = new();

        public int ProfileCompletionPercentage { get; set; }
        public List<string> MissingProfileItems { get; set; } = new();

        // ملاحظة: اربط هذه القيمة بمصدر بيانات Portfolio الفعلي عند توفره في المشروع
        public int PortfolioCount { get; set; } = 0;
        public string? StudentId { get; set; }

        /// <summary>
        /// يحدد ما إذا كان صاحب الجلسة الحالية هو نفسه صاحب البروفايل المعروض.
        /// إذا كانت true: يمكن التعديل. إذا كانت false: عرض فقط (Read-only).
        /// </summary>
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(string? studentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            Project.Models.Student? student;

            if (!string.IsNullOrWhiteSpace(studentId))
            {
                // فتح بروفايل طالب آخر عبر الرابط -> وضع عرض فقط ما لم يكن هو صاحب الحساب نفسه
                student = await _studentsRepository.GetStudentByUserId(studentId);
                StudentId = studentId;
            }
            else
            {
                // فتح المستخدم لبروفايله الخاص عبر القائمة/الحساب -> قابل للتعديل
                student = await _studentsRepository.GetStudentByUserId(userId);
            }

            if (student == null)
                return NotFound();

            CurrentStudent = student;

            // صاحب البروفايل فقط هو من يمكنه التعديل، بغض النظر عن الطريقة التي وصل بها للصفحة
            CanEdit = string.Equals(student.UserID, userId, StringComparison.OrdinalIgnoreCase);

            await LoadPageDataAsync();
            CalculateProfileCompletion();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("CurrentStudent.User");
            ModelState.Remove("CurrentStudent.Department");
            ModelState.Remove("CurrentStudent.University");
            ModelState.Remove("CurrentStudent.Specialty");

            var student = await _studentsRepository.GetStudentById(CurrentStudent.StudentID);

            if (student == null)
                return NotFound();

            // تحقق أمني: لا يُسمح بتعديل بروفايل لا يخص المستخدم الحالي
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !string.Equals(student.UserID, userId, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            // 1) التحقق من الامتداد والحجم قبل أي عملية حفظ
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var imageExt = Path.GetExtension(ProfileImage.FileName).ToLowerInvariant();

                if (!AllowedImageExtensions.Contains(imageExt))
                    ModelState.AddModelError(string.Empty, "صيغة الصورة غير مدعومة. الصيغ المسموحة: jpg, jpeg, png.");
                else if (ProfileImage.Length > MaxImageSizeBytes)
                    ModelState.AddModelError(string.Empty, "حجم الصورة يجب ألا يتجاوز 5MB.");
            }

            if (CVFile != null && CVFile.Length > 0)
            {
                var cvExt = Path.GetExtension(CVFile.FileName).ToLowerInvariant();

                if (!AllowedCvExtensions.Contains(cvExt))
                    ModelState.AddModelError(string.Empty, "صيغة ملف CV غير مدعومة. الصيغ المسموحة: pdf, doc, docx.");
                else if (CVFile.Length > MaxCvSizeBytes)
                    ModelState.AddModelError(string.Empty, "حجم ملف CV يجب ألا يتجاوز 10MB.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"{item.Key} : {error.ErrorMessage}");
                    }
                }

                CurrentStudent = student;
                CanEdit = true;
                await LoadPageDataAsync();
                CalculateProfileCompletion();
                return Page();
            }

            // 2) تحديث البيانات الأساسية
            student.Name = CurrentStudent.Name;
            student.PhoneNumber = CurrentStudent.PhoneNumber;
            student.Bio = CurrentStudent.Bio;
            student.SpecialtyID = CurrentStudent.SpecialtyID;
            student.Level = CurrentStudent.Level;



            // 3) رفع الصورة الشخصية (إنشاء المجلد -> حفظ الملف -> تحديث المسار)
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var oldImagePath = student.ProfileImagePath;

                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profile");
                Directory.CreateDirectory(imageFolder);

                var imageName = Guid.NewGuid() + Path.GetExtension(ProfileImage.FileName);
                var imagePath = Path.Combine(imageFolder, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }

                student.ProfileImagePath = "/uploads/profile/" + imageName;

                DeletePhysicalFile(oldImagePath);
            }

            // 4) رفع السيرة الذاتية (إنشاء المجلد -> حفظ الملف -> تحديث المسار)
            if (CVFile != null && CVFile.Length > 0)
            {
                var oldCvPath = student.CVPath;

                var cvFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/cv");
                Directory.CreateDirectory(cvFolder);

                var cvName = Guid.NewGuid() + Path.GetExtension(CVFile.FileName);
                var cvPath = Path.Combine(cvFolder, cvName);

                using (var stream = new FileStream(cvPath, FileMode.Create))
                {
                    await CVFile.CopyToAsync(stream);
                }

                student.CVPath = "/uploads/cv/" + cvName;

                DeletePhysicalFile(oldCvPath);
            }

            await _studentsRepository.SaveChangesAsync();

            TempData["Success"] = "تم تحديث البيانات بنجاح";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddSkillAsync()
        {
            var student = await _studentsRepository.GetStudentById(CurrentStudent.StudentID);

            if (student == null)
                return NotFound();

            // تحقق أمني: لا يُسمح بإضافة مهارة لبروفايل لا يخص المستخدم الحالي
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !string.Equals(student.UserID, userId, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            bool exists = await _context.StudentSkills.AnyAsync(x =>
                x.StudentID == student.StudentID &&
                x.SkillID == NewSkill.SkillID);

            if (exists)
            {
                TempData["Error"] = "هذه المهارة مضافة مسبقاً.";
                return RedirectToPage();
            }

            _context.StudentSkills.Add(new StudentSkill
            {
                StudentID = student.StudentID,
                SkillID = NewSkill.SkillID,
                Level = NewSkill.Level
            });

            await _studentsRepository.SaveChangesAsync();

            TempData["Success"] = "تمت إضافة المهارة.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteSkillAsync(int studentSkillId)
        {
            var studentSkill = await _context.StudentSkills
                .FirstOrDefaultAsync(x => x.StudentSkillID == studentSkillId);

            if (studentSkill == null)
                return RedirectToPage();

            // تحقق أمني: لا يُسمح بحذف مهارة تخص طالباً آخر
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var owningStudent = await _studentsRepository.GetStudentById(studentSkill.StudentID);

            if (string.IsNullOrEmpty(userId) ||
                owningStudent == null ||
                !string.Equals(owningStudent.UserID, userId, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            _context.StudentSkills.Remove(studentSkill);

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف المهارة.";

            return RedirectToPage();
        }

        private async Task LoadPageDataAsync()
        {
            Specialties = new SelectList(
                await _studentsRepository.GetSpecialtiesAsync(),
                "SpecialtyID", "Name", CurrentStudent.SpecialtyID);

            Skills = new SelectList(
                await _studentsRepository.GetSkillsAsync(),
                "SkillID", "Name");
        }

        private void CalculateProfileCompletion()
        {
            int percentage = 0;
            MissingProfileItems = new List<string>();

            if (!string.IsNullOrWhiteSpace(CurrentStudent.ProfileImagePath))
                percentage += 15;
            else
                MissingProfileItems.Add("إضافة صورة شخصية");

            if (!string.IsNullOrWhiteSpace(CurrentStudent.CVPath))
                percentage += 20;
            else
                MissingProfileItems.Add("رفع السيرة الذاتية (CV)");

            if (!string.IsNullOrWhiteSpace(CurrentStudent.Bio))
                percentage += 10;
            else
                MissingProfileItems.Add("إضافة نبذة شخصية");

            if (CurrentStudent.SpecialtyID > 0)
                percentage += 10;
            else
                MissingProfileItems.Add("اختيار التخصص");

            if (CurrentStudent.Skills != null && CurrentStudent.Skills.Count >= 3)
                percentage += 20;
            else
                MissingProfileItems.Add("إضافة 3 مهارات على الأقل");

            if (PortfolioCount > 0)
                percentage += 25;
            else
                MissingProfileItems.Add("إضافة مشروع إلى Portfolio");

            ProfileCompletionPercentage = percentage;
        }

        private static void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                relativePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}