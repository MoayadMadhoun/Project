using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Institution.ViewModels;

public class CreateOpportunityFromRequestVM
{
    public int RequestID { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    [Required]
    public int Capacity { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public int TermID { get; set; }

    [Required]
    public string Location { get; set; } = "";

    public List<OpportunitySpecialtyVM> Specialties { get; set; }
    = new();

    public List<OpportunitySkillVM> Skills { get; set; } = new();
}