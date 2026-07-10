namespace Project.Pages.Institution.ViewModels;

public class OpportunitySkillVM
{
    public int SkillID { get; set; }

    public string SkillName { get; set; } = "";

    public bool Selected { get; set; }

    public bool IsRequired { get; set; }
}