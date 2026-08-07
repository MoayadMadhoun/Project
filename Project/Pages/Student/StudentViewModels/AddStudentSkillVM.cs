using Project.Models;

namespace Project.Pages.Student.StudentViewModels
{
    public class AddStudentSkillVM
    {
        public int SkillID { get; set; }

        public StudentSkill.StudentSkillLevel Level { get; set; }
    }
}
