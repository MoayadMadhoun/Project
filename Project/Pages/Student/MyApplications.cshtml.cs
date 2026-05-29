using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Extensions;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    [Authorize]

    public class MyApplicationsModel : PageModel
    {
        private readonly StudentsRepository studentsRepository;

        public MyApplicationsModel(StudentsRepository StudentsRepository)
        {
            studentsRepository = StudentsRepository;
        }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        [BindProperty(SupportsGet = true)]

        public int PageSize { get; set; } = 5;
        public PaginatedList<TrainingApplication>? StudentApplications { get; set; }
        public int TotalApplicationCount => StudentApplications?.TotalCount ?? 0;
        public int CurrentApplicationCount => StudentApplications?.Count()  ?? 0;
        public async Task< IActionResult> OnGet()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userId is null)   return RedirectToPage("/Identity/Account/Login");
            var student = await studentsRepository.GetStudentByUserId(userId.ToString());
            if (student is null) return NotFound();
            var applications = studentsRepository.GetAllApplicationByStudentId(student.StudentID);
           
            StudentApplications =await  PaginatedList<TrainingApplication>.CreateAsync(applications, PageSize , PageIndex); 
            return Page();
        }
    }
}
