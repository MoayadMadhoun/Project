using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class PortfolioItem
    {
        [Key]
        public string PortfolioItemID { get; set; }
        [ForeignKey(nameof(StudentID))]
        public int StudentID { get; set; }
        public Student Student { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string Url { get; set; }
        public string ItemType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
