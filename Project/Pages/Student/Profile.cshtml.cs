using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class ProfileModel : PageModel
    {
        private readonly StudentsRepository _studentsRepository;

        public ProfileModel(
            StudentsRepository studentsRepository)
        {
            _studentsRepository = studentsRepository;
        }

        [BindProperty]
        public Project.Models.Student CurrentStudent { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return RedirectToPage(
                    "/Account/Login",
                    new { area = "Identity" });

            var student =
                await _studentsRepository
                .GetStudentByUserId(userId);

            if (student == null)
                return NotFound();

            CurrentStudent = student;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var student =
                await _studentsRepository
                .GetStudentById(CurrentStudent.StudentID);

            if (student == null)
                return NotFound();

            student.Name = CurrentStudent.Name;
            student.PhoneNumber = CurrentStudent.PhoneNumber;
            student.Bio = CurrentStudent.Bio;

            await _studentsRepository.SaveChangesAsync();

            TempData["Success"] =
                "تم تحديث البيانات بنجاح";

            return RedirectToPage();
        }
    }
}