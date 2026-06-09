using System.ComponentModel.DataAnnotations;

namespace Project.Pages.University.ViewModels
{
    public class EditSpecialtyVM
    {
        public int SpecialtyID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        public bool IsActive { get; set; }
    }
}
