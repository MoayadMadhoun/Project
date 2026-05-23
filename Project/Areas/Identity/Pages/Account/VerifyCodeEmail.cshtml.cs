
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Services;
using static System.Net.WebRequestMethods;

namespace Project.Areas.Identity.Pages.Account
{
    public class VerifyCodeEmailModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AspNetUser> _userManager;  
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly OptService _optService;
        private readonly IEmailSender _emailSender;

        public VerifyCodeEmailModel(
            ApplicationDbContext dbContext,
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            OptService optService,
            IEmailSender emailSender
            
        )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _optService = optService;
            _emailSender = emailSender;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty]  
        public string Code { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(Email))
            {
                
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Email is   null)
            {
                ModelState.AddModelError(nameof(Email), "Not Found An Email That is Error ");
                return Page();
            }
            var user = await _userManager.FindByEmailAsync(Email);


            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return Page();
            }

            var verification = _dbContext.EmailVerificationCodes
                .Where(x =>
                    x.UserId == user.Id &&
                    !x.IsUsed &&
                    x.ExpireAt > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (verification == null || verification.Code != Code)
            {
                ModelState.AddModelError("", "Invalid or expired code");
                return Page();
            }

            
            user.EmailConfirmed = true;
            verification.IsUsed = true;

            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            if (Email is null)
            {
                ModelState.AddModelError(nameof(Email), "Not Found An Email That is Error ");
                return Page();

            }
            var user = await _userManager.FindByEmailAsync(Email);


            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return Page();
            }

            //**Performance Loose
            //var oldCode = _dbContext
            //    .EmailVerificationCodes
            //    .Where(Ev => Ev.UserId == user.Id && !Ev.IsUsed);

            //foreach(var c in oldCode)
            //{
            //    c.IsUsed = true;
            //}


            // Hight Performance
            await _dbContext
                .EmailVerificationCodes
                .Where(Ev => Ev.UserId == user.Id && !Ev.IsUsed)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsUsed, true));

            var otp = _optService.GenerateOtp();

            var newCode = new EmailVerificationCode
            {
                UserId = user.Id,
                Code = otp,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed=false
            };

            await _dbContext.EmailVerificationCodes.AddAsync(newCode);

            await _dbContext.SaveChangesAsync();

            await _emailSender.SendEmailAsync(Email, "Your Veriviction Code is ", $"Your code is <h2>{otp}</h2>");

            return new JsonResult(new { success = true });
        }
       

    }


}