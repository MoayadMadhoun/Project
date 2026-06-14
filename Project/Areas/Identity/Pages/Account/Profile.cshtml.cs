using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Project.Models;


namespace Project.Areas.Identity.Pages.Account
{
   
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileModel(
            UserManager<AspNetUser> userManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        [BindProperty]
        public IFormFile? ProfileImage { get; set; }

        public string CurrentImage { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            CurrentImage = string.IsNullOrEmpty(user?.ProfileImagePath)
                ? "/images/default-user.jpg"
                : user.ProfileImagePath;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToPage();

            if (ProfileImage != null)
            {
                var folder =
                    Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "profiles");

                Directory.CreateDirectory(folder);

                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(ProfileImage.FileName);

                var path =
                    Path.Combine(folder, fileName);

                using var stream =
                    new FileStream(path, FileMode.Create);

                await ProfileImage.CopyToAsync(stream);

                user.ProfileImagePath =
                    $"/uploads/profiles/{fileName}";

                await _userManager.UpdateAsync(user);
            }

            return RedirectToPage();
        }
    }
}
