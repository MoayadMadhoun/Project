using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class PortfolioItem
    {
        [Key]
        public int PortfolioItemId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot be more than 300 characters")]
        [MinLength(2, ErrorMessage = "Title must be at least 2 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot be more than 1000 characters")]
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        [Url(ErrorMessage = "Invalid URL format")]
        public string? Url { get; set; }

        [Required(ErrorMessage = "Item type is required")]
        [RegularExpression("^(Project|Certificate|Report|Presentation|Link|Other)$",
            ErrorMessage = "Invalid item type")]
        public string ItemType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}
