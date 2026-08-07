// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Project.Areas.Identity.Pages.Account.RegisterVM;
using Project.Data;
using Project.Models;
using Project.Models.Enums;
using Project.Repositories;
using Project.Repository;
using Project.Repostory;
using Project.Services;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Project.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IUserStore<AspNetUser> _userStore;
        private readonly IUserEmailStore<AspNetUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UniversityRepository _universityRepository;
        private readonly TrainingInstitutionRepository _trainingInstitutionRepository;
        private readonly StudentsRepository _studentsRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly OptService _optService;

        public RegisterModel(
            UserManager<AspNetUser> userManager,
            IUserStore<AspNetUser> userStore,
            SignInManager<AspNetUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            UniversityRepository universityRepository,
            TrainingInstitutionRepository trainingInstitutionRepository,
            StudentsRepository studentsRepository,
            ApplicationDbContext dbContext,
            OptService optService

            )
        {
            ArgumentNullException.ThrowIfNull(studentsRepository);
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _universityRepository = universityRepository;
            _trainingInstitutionRepository = trainingInstitutionRepository;
            _studentsRepository = studentsRepository;
            _dbContext = dbContext;
            _optService = optService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }

        [BindProperty]
        public UniversityRegisterVM UniversityInput { get; set; }

        [BindProperty]
        public InistituationRegisterVM InistitutionInput { get; set; }

        [BindProperty]
        public StudentRegisterVM StudentInput { get; set; }

        public string UserRole { get; set; }

        [BindProperty(SupportsGet = true)]
        public AccountType AccountType { get; set; }


        public SelectList UniversityList { get; set; }

        public SelectList InstitutionTypeList { get; set; }
        public async Task OnGetAsync(string returnUrl = null)
        {

            UniversityList = await _universityRepository.CreateUniversitySelectList();
            InstitutionTypeList = EnumExtensions.GetEnumSelectList<InstitutionType>();

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public JsonResult OnGetDepartments(int universityId)
        {
            var departments = _dbContext
                .Departments
                .Where(d => d.UniversityID == universityId)
                .Select(d => new { id = d.DepartmentID, name = d.Name })
                .ToList();

            return new JsonResult(departments);
        }
        public JsonResult OnGetSpecialties(int departmentId)
        {
            var Specialties = _dbContext
                .Specialties
                .Where(s => s.DepartmentID == departmentId)
                .Select(s => new { id = s.SpecialtyID, name = s.Name })
                .ToList();

            return new JsonResult(Specialties);
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {


            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            RemoveUnusedValidation();
            UniversityList = await _universityRepository.CreateUniversitySelectList();
            InstitutionTypeList = EnumExtensions.GetEnumSelectList<InstitutionType>();
            foreach (var item in ModelState)
            {
                var field = item.Key;
                var errors = item.Value.Errors;

                foreach (var error in errors)
                {
                    Console.WriteLine($"{field}: {error.ErrorMessage}");
                }
            }
            if (ModelState.IsValid)
            {
                var user = new AspNetUser
                {
                    Email = Input.Email,
                    UserName = Input.Email,
                    IsActive = true,
                    AccountType = AccountType,
                    CreatedAt = DateTime.Now,

                };
                if (AccountType == AccountType.Institution)
                {
                    user.FullName = InistitutionInput.FullName;
                    user.PhoneNumber = InistitutionInput.PhoneNumber;
                    user.PhoneNumberConfirmed = true;

                    UserRole = "InstitutionTrainingOfficer";

                }
                if (AccountType == AccountType.University)
                {
                    user.FullName = UniversityInput?.FullName;
                    user.PhoneNumber = UniversityInput.PhoneNumber;
                    user.PhoneNumberConfirmed = true;

                    UserRole = "UniversityTrainingAdmin";

                }
                if (AccountType == AccountType.Student)
                {
                    user.FullName = StudentInput?.FullName;
                    user.PhoneNumber = StudentInput.PhoneNumber;
                    user.PhoneNumberConfirmed = true;

                    UserRole = "Student";

                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {


                    await _userManager.AddToRoleAsync(user, UserRole);

                    int? institutionId = null;
                    int? universityId = null;

                    if (AccountType == AccountType.Institution)
                    {
                        var institution = await CreateInstitutionAccount(user.Id);
                        institutionId = institution.InstituationID;
                    }

                    if (AccountType == AccountType.University)
                    {
                        var university = await CreateUniversityAccount(user.Id);
                        universityId = university.UniversityID;
                    }

                    if (AccountType == AccountType.Student)
                    {
                        await CreateStudentAccount(user.Id);
                    }

                    await CreateRoleScope(AccountType, user.Id, institutionId, universityId);


                    _logger.LogInformation("User created a new account with password.");

                    //var userId = await _userManager.GetUserIdAsync(user);
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    var otp = _optService.GenerateOtp();

                    _dbContext.EmailVerificationCodes.Add(new EmailVerificationCode
                    {
                        UserId = user.Id,
                        Code = otp,
                        ExpireAt = DateTime.UtcNow.AddMinutes(5),
                        CreatedAt = DateTime.Now,
                        IsUsed = false

                    });
                   
                    await _dbContext.SaveChangesAsync();

                    var emailBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<title>Verify Your Spaceara Account</title>
</head>

<body style='margin:0;padding:0;background-color:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='background:#f4f7fb;padding:40px 0;'>
<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:16px;overflow:hidden;
box-shadow:0 8px 24px rgba(0,0,0,.08);'>

<!-- Header -->
<tr>
<td style='background:#00b793;padding:35px;text-align:center;'>

<h1 style='margin:0;color:#ffffff;font-size:34px;font-weight:bold;'>
 Spaceara
</h1>

<p style='margin-top:10px;color:#d9fff8;font-size:16px;'>
Field Training Management Platform
</p>

</td>
</tr>

<!-- Content -->
<tr>
<td style='padding:40px;'>

<h2 style='margin-top:0;color:#1e293b;'>
Hello {user.FullName},
</h2>

<p style='font-size:16px;color:#475569;line-height:1.8;'>
Welcome to <strong style='color:#00b793;'>Spaceara</strong> —
the smart platform connecting
students, universities, and training institutions.
</p>

<p style='font-size:16px;color:#475569;line-height:1.8;'>
Please use the verification code below to activate your account:
</p>

<div style='margin:35px 0;text-align:center;'>

<div style='display:inline-block;
background:#e8fffa;
border:2px dashed #00b793;
padding:18px 40px;
border-radius:12px;'>

<span style='font-size:34px;
font-weight:bold;
letter-spacing:8px;
color:#00b793;'>
{otp}
</span>

</div>

</div>

<p style='font-size:15px;color:#ef4444;font-weight:600;'>
⏳ This verification code will expire in 10 minutes.
</p>

<p style='font-size:15px;color:#64748b;line-height:1.8;'>
If you didn't create a Spaceara account, you can safely ignore this email.
No further action is required.
</p>

</td>
</tr>

<!-- Footer -->
<tr>
<td style='background:#f8fafc;padding:30px;text-align:center;border-top:1px solid #e2e8f0;'>

<p style='margin:0;color:#334155;font-size:16px;font-weight:bold;'>
Thank you for choosing Spaceara ❤️
</p>

<p style='margin:12px 0 0;color:#64748b;font-size:14px;'>
Connecting Students • Universities • Training Institutions
</p>

<p style='margin-top:18px;color:#94a3b8;font-size:13px;'>
© {DateTime.Now.Year} Spaceara. All rights reserved.
</p>

</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>
";

                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        " Verify your Spaceara Account",
                        emailBody
                    );
                    return RedirectToPage("/Account/VerifyCodeEmail", new { email = Input.Email,accountType=AccountType });


                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return Page();


        


        }

        private AspNetUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AspNetUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AspNetUser)}'. " +
                    $"Ensure that '{nameof(AspNetUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

      

        private IUserEmailStore<AspNetUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AspNetUser>)_userStore;
        }

        private async Task<TrainingInstitution> CreateInstitutionAccount(string userId)
        {
            var institution = new TrainingInstitution
            {
                Address = InistitutionInput.Address,
                Name = InistitutionInput.FullName,
                Email = Input.Email,
                PhoneNumber = InistitutionInput.PhoneNumber,
                IsActive = true,
                UserID = userId,
                InstitutionType = Enum.Parse<InstitutionType>(InistitutionInput.InstituationType)
            };

            await _trainingInstitutionRepository.AddAsync(institution);

            return institution;
        }

        private async Task<University> CreateUniversityAccount(string userId)
        {
            var university = new University
            {
                Name = UniversityInput.FullName,
                Address = UniversityInput.Address,
                Email = Input.Email,
                PhoneNumber = UniversityInput.PhoneNumber,
                IsActive = true,
                UserID = userId
            };

            await _universityRepository.AddAsync(university);

            return university;
        }

        private async Task<Student> CreateStudentAccount(string userId)
        {
            var student = new Student
            {
                Name = StudentInput.FullName,
                StudentNumber = StudentInput.StudentNumber,
                DepartmentID = StudentInput.DepartmentID,
                SpecialtyID = StudentInput.SpecialtyId,
                Status = Student.StudentStatus.Active,
                UniversityID = StudentInput.UniversityID,
                PhoneNumber = StudentInput.PhoneNumber,
                UserID = userId
            };

            await _studentsRepository.AddAsync(student);

            return student;
        }

     

        private async Task CreateRoleScope(
     AccountType type,
     string userId,
     int? institutionId = null,
     int? universityId = null)
        {
            var scope = new AspNetRoleScope
            {
                UserID = userId,
                IsActive = true
            };

            switch (type)
            {
                case AccountType.Institution:
                    scope.RoleID = "4";
                    scope.InstitutionID = institutionId;
                    break;

                case AccountType.University:
                    scope.RoleID = "1";
                    scope.UniversityID = universityId;
                    break;

                case AccountType.Student:
                    scope.RoleID = "6";
                    scope.UniversityID = StudentInput.UniversityID;
                    scope.DepartmentID = StudentInput.DepartmentID;
                    break;
            }

            _dbContext.AspNetRoleScopes.Add(scope);
            await _dbContext.SaveChangesAsync();
        }


        private void RemoveUnusedValidation()
        {
            ModelState.Remove("Address");
            ModelState.Remove("FullName");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("StudentNumber");
            ModelState.Remove("InstituationType");
        }
















    }
}


