
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Models.Enums;
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

        [BindProperty(SupportsGet =true)]
        public AccountType AccountType { get; set; }


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

            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                return RedirectToPage("/Student/Index");
            }

            if (await _userManager.IsInRoleAsync(user, "UniversityTrainingAdmin") ||
                await _userManager.IsInRoleAsync(user, "DepartmentHead") ||
                await _userManager.IsInRoleAsync(user, "UniversitySupervisor"))
            {
                return RedirectToPage("/University/Index");
            }

            if (await _userManager.IsInRoleAsync(user, "InstitutionTrainingOfficer") ||
                await _userManager.IsInRoleAsync(user, "InstitutionSupervisor"))
            {
                return RedirectToPage("/Institution/Index");
            }

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

            await _emailSender.SendEmailAsync(Email, " 🚀 Verify your SpaceArea account", $"Dear<b> {user.FullName }</b><br />,Welcome to SpaceArea – the smart hub connecting students, universities, and training institutions!<br />  Please use the 6-digit verification code below to confirm your email address and activate your account:<br /><h2>  Verification Code : {otp}</h2><br />  ⏳ Important: This code is valid for 10 minutes only.<br /> If you didn't request this email, you can safely ignore it.<br /> Your account security is our priority.<br /><h2> Best regards, The SpaceArea Team</h2>");
            //await _emailSender.SendEmailAsync(Email, " 🚀 Verify your SpaceArea account", $"Your code is <h2>{otp}</h2>");

            return new JsonResult(new { success = true });
        }
       

    }


}