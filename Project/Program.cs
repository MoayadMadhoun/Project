using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Options;
using Project.Repositories;
using Project.Repository;
using Project.Repostory;
using Project.Services;

namespace Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<AspNetUser>(options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;

            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/AccessDenied";
            });

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SMTP"));
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddScoped<TrainingInstitutionRepository>();
            builder.Services.AddScoped<UniversityRepository>();
            builder.Services.AddScoped<StudentsRepository>();
            builder.Services.AddScoped<DepartmentRepository>();
            builder.Services.AddScoped<SpecialtyRepository>();
            builder.Services.AddScoped<TrainingOpportunityRepository>();
            builder.Services.AddScoped<OptService>();
            builder.Services.AddScoped<CreateUserService>();

            builder.Services.AddKeyedScoped<IUploadFils, UploadDocxFile >("file");
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
