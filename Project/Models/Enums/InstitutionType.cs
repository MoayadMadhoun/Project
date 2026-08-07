using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project.Models.Enums
{
    public enum InstitutionType
    {
        
        [Display(Name = "بنك")]
        Bank = 1,

        [Display(Name = "مستشفى")]
        Hospital = 2,

        [Display(Name = "شركة برمجيات")]
        SoftwareCompany = 3,

        [Display(Name = "مدرسة")]
        School = 4,

        [Display(Name = "جامعة")]
        University = 5,

        [Display(Name = "شركة اتصالات")]
        TelecommunicationsCompany = 6,

        [Display(Name = "مؤسسة حكومية")]
        GovernmentOrganization = 7,

        [Display(Name = "شركة هندسية")]
        EngineeringCompany = 8,

        [Display(Name = "مركز تدريب")]
        TrainingCenter = 9,

        [Display(Name = "منظمة غير ربحية")]
        NonProfitOrganization = 10



     
    }
}
