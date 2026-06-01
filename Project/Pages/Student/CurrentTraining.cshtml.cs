using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data;
using Project.Models;
using Project.Repositories;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class CurrentTrainingModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly StudentsRepository studentsRepository;

        public TrainingPlacement? CurrentPlacement { get; set; }
      
        public CurrentTrainingModel(ApplicationDbContext context , StudentsRepository studentsRepository)
        {
           _context = context;
            this.studentsRepository = studentsRepository;
        }
        public string GetTrainingDuration(TrainingPlacement? CurrentPlacement)
         {
            if (CurrentPlacement is null) return "";
            int days =  (CurrentPlacement.EndDate - CurrentPlacement.StartDate).Days;
            if (days == 0) { return $""; }

            if (days < 30) { return $"{days} يوم"; }
          
            return $"{days / 30} شهر  "; 
          
        }
        public string TrainingDuration { get; set; }
        public async Task <IActionResult> OnGet()
            
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) { return RedirectToPage("/Identity/Account/Login"); }
            var student = studentsRepository.GetStudentByUserId(userId);
            if (student is null) return Forbid();
            CurrentPlacement = await studentsRepository.GetTrainingPlacementByStudentId(student.Id);
             TrainingDuration = GetTrainingDuration(CurrentPlacement);
            return Page();
        }
    }
}
