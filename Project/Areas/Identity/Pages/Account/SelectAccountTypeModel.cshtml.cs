using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Areas.Identity.Pages.Account
{
    public class SelectAccountTypeModelModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirect already-authenticated users away from this page
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToPage("/Index");

            return Page();
        }
    }
}
