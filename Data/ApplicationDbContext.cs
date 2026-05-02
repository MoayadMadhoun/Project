using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data;

public class ApplicationDbContext : IdentityDbContext
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Uniqe Constrains

        builder.Entity<Specialty>()
            .HasIndex(s => s.Name).IsUnique();

        builder.Entity<Skill>()
            .HasIndex(s => s.Name).IsUnique();

        builder.Entity<Student>()
            .HasIndex(s => s.UserID).IsUnique();

        builder.Entity<Student>()
            .HasIndex(s => s.StudentNumber).IsUnique();

        builder.Entity<StudentSkill>()
            .HasIndex(ss => new { ss.StudentID, ss.SkillID }).IsUnique();

        builder.Entity<RequestSpecialty>()
            .HasIndex(rs => new { rs.RequestID, rs.SpecialtyID }).IsUnique();

        builder.Entity<RequestSkill>()
            .HasIndex(rs => new { rs.RequestID, rs.SkillID }).IsUnique();

        builder.Entity<OpportunitySpecialty>()
            .HasIndex(os => new { os.OpportunityID, os.SpecialtyID }).IsUnique();

        builder.Entity<OpportunitySkill>()
            .HasIndex(os => new { os.OpportunityID, os.SkillID }).IsUnique();

        builder.Entity<TrainingApplication>()
            .HasIndex(a => new { a.OpportunityID, a.StudentID }).IsUnique();

        builder.Entity<TrainingPlacement>()
            .HasIndex(p => p.ApplicationID).IsUnique();

        builder.Entity<AttendanceRecord>()
            .HasIndex(a => new { a.PlacementID, a.AttendanceDate }).IsUnique();


        // RELATIONSHIPS — disable cascade where multiple FK paths would cause cycles


        // TrainingOpportunityRequest ->  UniversityAdmin
        builder.Entity<TrainingOpportunityRequest>()
            .HasOne(r => r.UniversityAdmin)
            .WithMany()
            .HasForeignKey(r => r.UniversityAdminID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingOpportunityRequest -> DepartmentHead
        builder.Entity<TrainingOpportunityRequest>()
            .HasOne(r => r.DepartmentHead)
            .WithMany()
            .HasForeignKey(r => r.DepartmentHeadID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingApplication -> DepartmentHead
        builder.Entity<TrainingApplication>()
            .HasOne(a => a.DepartmentHead)
            .WithMany()
            .HasForeignKey(a => a.DepartmentHeadID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingApplication -> UniversityAdmin
        builder.Entity<TrainingApplication>()
            .HasOne(a => a.UniversityAdmin)
            .WithMany()
            .HasForeignKey(a => a.UniversityAdminID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingApplication -> InstitutionOfficer
        builder.Entity<TrainingApplication>()
            .HasOne(a => a.InstitutionOfficer)
            .WithMany()
            .HasForeignKey(a => a.InstitutionOfficerID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingPlacement -> UniversitySupervisor
        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(p => p.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingPlacement -> InstitutionSupervisor
        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.InstitutionSupervisor)
            .WithMany()
            .HasForeignKey(p => p.InstitutionSupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingPlacement -> Student (restrict to avoid cycle via Application)
        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingPlacement -> Opportunity (restrict)
        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.TrainingOpportunity)
            .WithMany()
            .HasForeignKey(p => p.OpportunityID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingPlacement -> Institution (restrict)
        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.TrainingInstitution)
            .WithMany()
            .HasForeignKey(p => p.InstitutionID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingPlacement -> Term (restrict)
        builder.Entity<TrainingPlacement>()
            .HasOne(p => p.TrainingTerm)
            .WithMany()
            .HasForeignKey(p => p.TermID)
            .OnDelete(DeleteBehavior.Restrict);

        // AttendanceRecord -> InstitutionSupervisor
        builder.Entity<AttendanceRecord>()
            .HasOne(a => a.InstitutionSupervisor)
            .WithMany()
            .HasForeignKey(a => a.InstitutionSupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // FieldVisit -> UniversitySupervisor
        builder.Entity<FieldVisit>()
            .HasOne(v => v.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(v => v.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentEvaluation -> UniversitySupervisor
        builder.Entity<StudentEvaluation>()
            .HasOne(e => e.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(e => e.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentEvaluation -> InstitutionSupervisor
        builder.Entity<StudentEvaluation>()
            .HasOne(e => e.InstitutionSupervisor)
            .WithMany()
            .HasForeignKey(e => e.InstitutionSupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentReport -> UniversitySupervisor
        builder.Entity<StudentReport>()
            .HasOne(r => r.UniversitySupervisor)
            .WithMany()
            .HasForeignKey(r => r.UniversitySupervisorID)
            .OnDelete(DeleteBehavior.Restrict);

        // StudentReport -> Student (restrict to avoid cycle via Placement)
        builder.Entity<StudentReport>()
            .HasOne(r => r.Student)
            .WithMany(s => s.Reports)
            .HasForeignKey(r => r.StudentID)
            .OnDelete(DeleteBehavior.Restrict);

        // TrainingOpportunity -> InstitutionOfficer
        builder.Entity<TrainingOpportunity>()
            .HasOne(o => o.InstitutionOfficer)
            .WithMany()
            .HasForeignKey(o => o.InstitutionOfficerID)
            .OnDelete(DeleteBehavior.Restrict);

        // UserRoleScope -> Role
        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.Role)
            .WithMany()
            .HasForeignKey(s => s.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

        // UserRoleScope -> University
        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.University)
            .WithMany()
            .HasForeignKey(s => s.UniversityID)
            .OnDelete(DeleteBehavior.Restrict);

        // UserRoleScope -> Department
        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.Department)
            .WithMany()
            .HasForeignKey(s => s.DepartmentID)
            .OnDelete(DeleteBehavior.Restrict);

        // UserRoleScope -> Institution
        builder.Entity<AspNetRoleScope>()
            .HasOne(s => s.TrainingInstitution)
            .WithMany()
            .HasForeignKey(s => s.InstitutionID)
            .OnDelete(DeleteBehavior.Restrict);



        var roles = new[]
        {
            new IdentityRole { Id = "1", Name = "UniversityTrainingAdmin",   NormalizedName = "UNIVERSITYTRAININGADMIN" },
            new IdentityRole { Id = "2", Name = "DepartmentHead",            NormalizedName = "DEPARTMENTHEAD" },
            new IdentityRole { Id = "3", Name = "UniversitySupervisor",      NormalizedName = "UNIVERSITYSUPERVISOR" },
            new IdentityRole { Id = "4", Name = "InstitutionTrainingOfficer",NormalizedName = "INSTITUTIONTRAININGOFFICER" },
            new IdentityRole { Id = "5", Name = "InstitutionSupervisor",     NormalizedName = "INSTITUTIONSUPERVISOR" },
            new IdentityRole { Id = "6", Name = "Student",                   NormalizedName = "STUDENT" },
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }
}
