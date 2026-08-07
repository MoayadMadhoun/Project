using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels
{

    public class CreateEvaluationVM
    {
        public int PlacementID { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string OpportunityTitle { get; set; } = string.Empty;

        public string InstitutionName { get; set; } = string.Empty;

        [Required]
        public StudentEvaluation.EvaluationType Type { get; set; }

        [Required]
        public DateTime EvaluationDate { get; set; } = DateTime.Today;

        [Required]
        public decimal Score { get; set; }

        [Required]
        public decimal MaxScore { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}
