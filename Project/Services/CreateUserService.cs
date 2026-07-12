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
            var commited = false;

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
                commited = true;

                var body = $@"
                <h2>مرحباً {user.FullName}</h2>

                <p>تم إنشاء حسابك بنجاح.</p>

                <p>
                    البريد الإلكتروني:
                    {user.Email}
                </p>

                <p>
                    كلمة المرور:
                    {password}
                </p>

                <p>
                    كود التفعيل:
                    <strong>{otp}</strong>
                </p>";

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
                if (!commited)
                {
                    await transaction.RollbackAsync();
                }
              
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
