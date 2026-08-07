using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Student.StudentViewModels
{

    // ============================================================
    // Main ViewModel - Student/Profile (Edit / Owner view)
    // ============================================================
        public class StudentProfileVM
        {
            public int StudentID { get; set; }

            public string Name { get; set; }

            public string? Bio { get; set; }

            public string? PhoneNumber { get; set; }

            public string? Level { get; set; }

            public int? SpecialtyID { get; set; }
        }
    }
