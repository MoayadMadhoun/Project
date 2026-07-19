using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTO;
using Project.Models;

namespace Project.Services
{
    public class CreateUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly OptService _otpService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CreateUserService(
            ApplicationDbContext context,
            UserManager<AspNetUser> userManager,
            IEmailSender emailSender,
            OptService otpService,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _otpService = otpService;
            _roleManager = roleManager;
        }

        public async Task<ServiceResult> CreateUserAsync(CreateUserRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "البريد الإلكتروني مستخدم مسبقاً"
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var password = GeneratePassword();

                var user = new AspNetUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = $"حدث خطأ أثناء إنشاء المستخدم: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                    };
                }

                var role = await _roleManager.FindByNameAsync(request.RoleName);
                if (role == null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "الرول غير موجود"
                    };
                }

                var roleResult = await _userManager.AddToRoleAsync(user,request.RoleName);
                if (!roleResult.Succeeded)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = string.Join(", ",
                            roleResult.Errors.Select(e => e.Description))
                    };
                }
                _context.AspNetRoleScopes.Add(
                    new AspNetRoleScope
                    {
                        UserID = user.Id,
                        RoleID = role.Id,

                        UniversityID = request.UniversityId,
                        DepartmentID = request.DepartmentId,
                        InstitutionID = request.InstitutionId,

                        IsActive = true
                });

                var otp = _otpService.GenerateOtp();
                _context.EmailVerificationCodes.Add(
                    new EmailVerificationCode
                    {
                        UserId = user.Id,
                        Code = otp,
                        CreatedAt = DateTime.UtcNow,
                        ExpireAt = DateTime.UtcNow.AddMinutes(10),
                        IsUsed = false
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var body = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
</head>

<body style='margin:0;padding:0;background:#f4f7fb;font-family:Segoe UI,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;background:#f4f7fb;'>
<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:16px;overflow:hidden;box-shadow:0 8px 24px rgba(0,0,0,.08);'>

<tr>
<td style='background:#00b793;padding:30px;text-align:center;'>

<h1 style='margin:0;color:white;'>Spaceara</h1>

</td>
</tr>

<tr>
<td style='padding:40px;'>

<h2 style='margin-top:0;color:#1e293b;'>
مرحباً {user.FullName}
</h2>

<p style='font-size:16px;color:#475569;line-height:1.8;'>
تم إنشاء حسابك بنجاح.
</p>

<div style='background:#f8fafc;border:1px solid #e2e8f0;border-radius:10px;padding:20px;margin-top:20px;'>

<p style='margin:0 0 15px;color:#1e293b;font-size:16px;'>
<strong>البريد الإلكتروني:</strong><br>
{user.Email}
</p>

<p style='margin:0 0 15px;color:#1e293b;font-size:16px;'>
<strong>كلمة المرور:</strong><br>
{password}
</p>

</div>

<p style='margin-top:30px;font-size:16px;color:#475569;'>
كود التفعيل:
</p>

<div style='text-align:center;margin-top:15px;'>

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

</td>
</tr>

<tr>
<td style='background:#f8fafc;border-top:1px solid #e2e8f0;padding:20px;text-align:center;'>

<p style='margin:0;color:#64748b;font-size:14px;'>
© {DateTime.Now.Year} Spaceara. All rights reserved.
</p>

</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "بيانات الدخول",
                    body);


                return new ServiceResult
                {
                    Success = true,
                    Message = "تم إنشاء المستخدم بنجاح"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult
                {
                    Success = false,
                    Message = $"حدث خطأ أثناء إنشاء المستخدم: {ex.Message}"
                };
            }

        }
        private string GeneratePassword()
        {
            return $"Sp@{Random.Shared.Next(100000, 999999)}";
        }

    }
}
