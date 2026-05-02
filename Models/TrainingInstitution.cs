using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class TrainingInstitution
    {
        [Key]
        public int InstituationID {  get; set; }
        public string Name { get;  set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ContactPersonName { get; set; }
        public bool IsActive {  get; set; }
        public ICollection<TrainingOpportunity> TrainingOpportunities { get; set; }



    }
}
