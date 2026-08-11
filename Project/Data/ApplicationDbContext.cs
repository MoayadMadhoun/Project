using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data;

public class ApplicationDbContext : IdentityDbContext<AspNetUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<AspNetRoleScope> AspNetRoleScopes { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<FieldVisit> FieldVisits { get; set; }
    public DbSet<OpportunityRequestInstitution> OpportunityRequestInstitutions { get; set; }
    public DbSet<OpportunitySkill> OpportunitySkills { get; set; }
    public DbSet<OpportunitySpecialty> OpportunitySpecialties { get; set; }
    public DbSet<PortfolioItem> PortfolioItems { get; set; }
    public DbSet<RequestSkill> RequestSkills { get; set; }
    public DbSet<RequestSpecialty> RequestSpecialties { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentEvaluation> StudentEvaluations { get; set; }
    public DbSet<StudentReport> StudentReports { get; set; }
    public DbSet<StudentSkill> StudentSkills { get; set; }
    public DbSet<TrainingApplication> TrainingApplications { get; set; }
    public DbSet<TrainingInstitution> TrainingInstitutions { get; set; }
    public DbSet<TrainingOpportunity> TrainingOpportunities { get; set; }
    public DbSet<TrainingOpportunityRequest> TrainingOpportunityRequests { get; set; }
    public DbSet<TrainingPlacement> TrainingPlacements { get; set; }
    public DbSet<TrainingTerm> TrainingTerms { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =========================
        // UNIQUE CONSTRAINTS
        // =========================

        builder.Entity<Specialty>()
            .HasIndex(s => s.Name)
            .IsUnique();

        builder.Entity<Skill>()
            .HasIndex(s => s.Name)
            .IsUnique();

        builder.Entity<Student>()
            .HasIndex(s => s.UserID)
            .IsUnique();

        builder.Entity<Student>()
            .HasIndex(s => s.StudentNumber)
            .IsUnique();

        builder.Entity<StudentSkill>()
            .HasIndex(ss => new { ss.StudentID, ss.SkillID })
            .IsUnique();

        builder.Entity<RequestSpecialty>()
            .HasIndex(rs => new { rs.RequestID, rs.SpecialtyID })
            .IsUnique();

        builder.Entity<RequestSkill>()
            .HasIndex(rs => new { rs.RequestID, rs.SkillID })
            .IsUnique();

        builder.Entity<OpportunitySpecialty>()
            .HasIndex(os => new { os.OpportunityID, os.SpecialtyID })
            .IsUnique();

        builder.Entity<OpportunitySkill>()
            .HasIndex(os => new { os.OpportunityID, os.SkillID })
            .IsUnique();

        builder.Entity<TrainingApplication>()
            .HasIndex(a => new { a.OpportunityID, a.StudentID })
            .IsUnique();

        builder.Entity<TrainingPlacement>()
            .HasIndex(p => p.ApplicationID)
            .IsUnique();

        builder.Entity<AttendanceRecord>()
            .HasIndex(a => new { a.PlacementID, a.AttendanceDate })
            .IsUnique();

        // =========================
        // ENUM TO STRING CONVERSIONS
        // =========================

        builder.Entity<Student>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<FieldVisit>()
            .Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<StudentReport>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<StudentReport>()
            .Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<TrainingApplication>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<TrainingApplication>()
            .Property(s => s.DepartmentDecision)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<TrainingApplication>()
            .Property(s => s.UniversityAdminDecision)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<TrainingApplication>()
            .Property(s => s.InstitutionDecision)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<TrainingOpportunity>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);


        builder.Entity<TrainingOpportunityRequest>()
            .Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<TrainingPlacement>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<AttendanceRecord>()
            .Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<PortfolioItem>()
            .Property(p => p.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<OpportunityRequestInstitution>()
            .Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<StudentSkill>()
            .Property(s => s.Level)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<StudentEvaluation>()
            .Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<StudentEvaluation>()
            .Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Entity<Notification>()
            .HasIndex(n => n.UserId);

        builder.Entity<Notification>()
            .HasIndex(n => n.IsRead);

        builder.Entity<Notification>()
            .HasIndex(n => n.CreatedAt);

        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Notification>()
            .HasOne(n => n.Sender)
            .WithMany()
            .HasForeignKey(n => n.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // DECIMAL PRECISION
        // =========================

        builder.Entity<Student>()
            .Property(x => x.GPA)
            .HasPrecision(5, 2);

        builder.Entity<StudentEvaluation>()
            .Property(x => x.Score)
            .HasPrecision(5, 2);

        builder.Entity<StudentEvaluation>()
            .Property(x => x.MaxScore)
            .HasPrecision(5, 2);

        builder.Entity<FieldVisit>()
            .Property(x => x.Score)
            .HasPrecision(5, 2);

        builder.Entity<FieldVisit>()
            .Property(x => x.MaxScore)
            .HasPrecision(5, 2);

        // =========================
        // RELATIONSHIPS
        // =========================

        builder.Entity<TrainingOpportunity>()
            .HasOne(o => o.TrainingTerm)
            .WithMany()
            .HasForeignKey(o => o.TermID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingOpportunityRequest>()
            .HasOne(r => r.UniversityAdmin)
            .WithMany()
            .HasForeignKey(r => r.UniversityAdminID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingOpportunityRequest>()
            .HasOne(r => r.DepartmentHead)
            .WithMany()
            .HasForeignKey(r => r.DepartmentHeadID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingApplication>()
            .HasOne(a => a.DepartmentHead)
            .WithMany()
            .HasForeignKey(a => a.DepartmentHeadID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingApplication>()
            .HasOne(a => a.UniversityAdmin)
            .WithMany()
            .HasForeignKey(a => a.UniversityAdminID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingApplication>()
            .HasOne(a => a.InstitutionOfficer)
            .WithMany()
            .HasForeignKey(a => a.InstitutionOfficerID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingApplication>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingApplication>()
            .HasOne(a => a.TrainingOpportunity)
            .WithMany()
            .HasForeignKey(a => a.OpportunityID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingPlacement>()
        .HasOne(p => p.TrainingOpportunity)
        .WithMany(o => o.TrainingPlacement)
        .HasForeignKey(p => p.OpportunityID)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.InstitutionSupervisor)
            .WithMany()
            .HasForeignKey(p => p.InstitutionSupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentID)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.TrainingInstitution)
            .WithMany()
            .HasForeignKey(p => p.InstitutionID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.TrainingTerm)
            .WithMany()
            .HasForeignKey(p => p.TermID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AttendanceRecord>()
            .HasOne(a => a.InstitutionSupervisor)
            .WithMany()
            .HasForeignKey(a => a.InstitutionSupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FieldVisit>()
            .HasOne(v => v.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(v => v.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentEvaluation>()
            .HasOne(e => e.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(e => e.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentEvaluation>()
            .HasOne(e => e.InstitutionSupervisor)
            .WithMany()
            .HasForeignKey(e => e.InstitutionSupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentReport>()
            .HasOne(r => r.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(r => r.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StudentReport>()
            .HasOne(r => r.Student)
            .WithMany(s => s.Reports)
            .HasForeignKey(r => r.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TrainingOpportunity>()
            .HasOne(o => o.InstitutionOfficer)
            .WithMany()
            .HasForeignKey(o => o.InstitutionOfficerID)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.Role)
            .WithMany()
            .HasForeignKey(s => s.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.University)
            .WithMany()
            .HasForeignKey(s => s.UniversityID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.Department)
            .WithMany()
            .HasForeignKey(s => s.DepartmentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.TrainingInstitution)
            .WithMany()
            .HasForeignKey(s => s.InstitutionID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Student>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Department>()
            .HasOne(d => d.University)
            .WithMany(u => u.Departments)
            .HasForeignKey(d => d.UniversityID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<OpportunityRequestInstitution>()
            .HasOne(o => o.TrainingInstitution)
            .WithMany()
            .HasForeignKey(o => o.InstitutionID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Student>()
            .HasOne(s => s.University)
            .WithMany(u => u.Students)
            .HasForeignKey(s => s.UniversityID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Specialty>()
        .HasOne(s => s.Department)
        .WithMany(d => d.Specialties)
        .HasForeignKey(s => s.DepartmentID)
        .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<TrainingInstitution>()
            .Property(x => x.InstitutionType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // =========================
        // SEED ROLES
        // =========================

        var roles = new[]
        {
            new IdentityRole
            {
                Id = "1",
                Name = "UniversityTrainingAdmin",
                NormalizedName = "UNIVERSITYTRAININGADMIN",
                ConcurrencyStamp = "1"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "DepartmentHead",
                NormalizedName = "DEPARTMENTHEAD",
                ConcurrencyStamp = "2"
            },
            new IdentityRole
            {
                Id = "3",
                Name = "UniversitySupervisor",
                NormalizedName = "UNIVERSITYSUPERVISOR",
                ConcurrencyStamp = "3"
            },
            new IdentityRole
            {
                Id = "4",
                Name = "InstitutionTrainingOfficer",
                NormalizedName = "INSTITUTIONTRAININGOFFICER",
                ConcurrencyStamp = "4"
            },
            new IdentityRole
            {
                Id = "5",
                Name = "InstitutionSupervisor",
                NormalizedName = "INSTITUTIONSUPERVISOR",
                ConcurrencyStamp = "5"
            },
            new IdentityRole
            {
                Id = "6",
                Name = "Student",
                NormalizedName = "STUDENT",
                ConcurrencyStamp = "6"
            }
        };

        builder.Entity<IdentityRole>().HasData(roles);
    }
}