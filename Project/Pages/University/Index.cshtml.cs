using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Repostory;
using Project.Models;
using Microsoft.AspNetCore.Identity;

namespace Project.Pages.University
{
    public class IndexModel : PageModel
    {
        //private readonly UniversityRepository _university;
        //private readonly SignInManager<AspNetUser> _signInManager;

        public IndexModel()
        {
         //   this._university = university;
           // this._signInManager = signInManager;
        }

       // public Models.University LogedUniversity { get; set; } 
        public void OnGet()
        {
        }
    }
}
