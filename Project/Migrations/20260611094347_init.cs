using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingTerms",
                columns: table => new
                {
                    TermID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTerms", x => x.TermID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerificationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerificationCodes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingInstitutions",
                columns: table => new
                {
                    InstituationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstitutionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ContactPersonName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingInstitutions", x => x.InstituationID);
                    table.ForeignKey(
                        name: "FK_TrainingInstitutions_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    UniversityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.UniversityID);
                    table.ForeignKey(
                        name: "FK_Universities_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UniversityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Departments_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleScopes",
                columns: table => new
                {
                    UserRoleScopeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UniversityID = table.Column<int>(type: "int", nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    InstitutionID = table.Column<int>(type: "int", nullable: true),
                    StudentID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleScopes", x => x.UserRoleScopeID);
                    table.ForeignKey(
                        name: "FK_AspNetRoleScopes_AspNetRoles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetRoleScopes_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetRoleScopes_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetRoleScopes_TrainingInstitutions_InstitutionID",
                        column: x => x.InstitutionID,
                        principalTable: "TrainingInstitutions",
                        principalColumn: "InstituationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetRoleScopes_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    SpecialtyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.SpecialtyID);
                    table.ForeignKey(
                        name: "FK_Specialties_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                });

            migrationBuilder.CreateTable(
                name: "TrainingOpportunityRequests",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedSeats = table.Column<int>(type: "int", nullable: false),
                    PreferredStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreferredEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    UniversityID = table.Column<int>(type: "int", nullable: false),
                    TermID = table.Column<int>(type: "int", nullable: false),
                    UniversityAdminID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartmentHeadID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingOpportunityRequests", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunityRequests_AspNetUsers_DepartmentHeadID",
                        column: x => x.DepartmentHeadID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunityRequests_AspNetUsers_UniversityAdminID",
                        column: x => x.UniversityAdminID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunityRequests_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_TrainingOpportunityRequests_TrainingTerms_TermID",
                        column: x => x.TermID,
                        principalTable: "TrainingTerms",
                        principalColumn: "TermID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunityRequests_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GPA = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CVPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    SpecialtyID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UniversityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_Students_Specialties_SpecialtyID",
                        column: x => x.SpecialtyID,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyID");
                    table.ForeignKey(
                        name: "FK_Students_Universities_UniversityID",
                        column: x => x.UniversityID,
                        principalTable: "Universities",
                        principalColumn: "UniversityID");
                });

            migrationBuilder.CreateTable(
                name: "OpportunityRequestInstitutions",
                columns: table => new
                {
                    RequestInstitutionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    InstitutionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityRequestInstitutions", x => x.RequestInstitutionID);
                    table.ForeignKey(
                        name: "FK_OpportunityRequestInstitutions_TrainingInstitutions_InstitutionID",
                        column: x => x.InstitutionID,
                        principalTable: "TrainingInstitutions",
                        principalColumn: "InstituationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityRequestInstitutions_TrainingOpportunityRequests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "TrainingOpportunityRequests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestSkills",
                columns: table => new
                {
                    RequestSkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSkills", x => x.RequestSkillID);
                    table.ForeignKey(
                        name: "FK_RequestSkills_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestSkills_TrainingOpportunityRequests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "TrainingOpportunityRequests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestSpecialties",
                columns: table => new
                {
                    RequestSpecialtyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    SpecialtyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSpecialties", x => x.RequestSpecialtyID);
                    table.ForeignKey(
                        name: "FK_RequestSpecialties_Specialties_SpecialtyID",
                        column: x => x.SpecialtyID,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestSpecialties_TrainingOpportunityRequests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "TrainingOpportunityRequests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingOpportunities",
                columns: table => new
                {
                    OpportunityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstitutionID = table.Column<int>(type: "int", nullable: false),
                    TermID = table.Column<int>(type: "int", nullable: false),
                    RequestID = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    InstitutionOfficerID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrainingTermTermID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingOpportunities", x => x.OpportunityID);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_AspNetUsers_InstitutionOfficerID",
                        column: x => x.InstitutionOfficerID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_TrainingInstitutions_InstitutionID",
                        column: x => x.InstitutionID,
                        principalTable: "TrainingInstitutions",
                        principalColumn: "InstituationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_TrainingOpportunityRequests_RequestID",
                        column: x => x.RequestID,
                        principalTable: "TrainingOpportunityRequests",
                        principalColumn: "RequestID");
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_TrainingTerms_TermID",
                        column: x => x.TermID,
                        principalTable: "TrainingTerms",
                        principalColumn: "TermID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_TrainingTerms_TrainingTermTermID",
                        column: x => x.TrainingTermTermID,
                        principalTable: "TrainingTerms",
                        principalColumn: "TermID");
                });

            migrationBuilder.CreateTable(
                name: "PortfolioItems",
                columns: table => new
                {
                    PortfolioItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItems", x => x.PortfolioItemID);
                    table.ForeignKey(
                        name: "FK_PortfolioItems_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSkills",
                columns: table => new
                {
                    StudentSkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkills", x => x.StudentSkillID);
                    table.ForeignKey(
                        name: "FK_StudentSkills_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSkills_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpportunitySkills",
                columns: table => new
                {
                    OpportunitySkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunitySkills", x => x.OpportunitySkillID);
                    table.ForeignKey(
                        name: "FK_OpportunitySkills_Skills_SkillID",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunitySkills_TrainingOpportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "TrainingOpportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpportunitySpecialties",
                columns: table => new
                {
                    OpportunitySpecialtyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityID = table.Column<int>(type: "int", nullable: false),
                    SpecialtyID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunitySpecialties", x => x.OpportunitySpecialtyID);
                    table.ForeignKey(
                        name: "FK_OpportunitySpecialties_Specialties_SpecialtyID",
                        column: x => x.SpecialtyID,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunitySpecialties_TrainingOpportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "TrainingOpportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingApplications",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentDecision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentHeadID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartmentReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UniversityAdminDecision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UniversityAdminID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UniversityAdminReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UniversityAdminNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InstitutionDecision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstitutionOfficerID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InstitutionReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstitutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingApplications", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_TrainingApplications_AspNetUsers_DepartmentHeadID",
                        column: x => x.DepartmentHeadID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingApplications_AspNetUsers_InstitutionOfficerID",
                        column: x => x.InstitutionOfficerID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingApplications_AspNetUsers_UniversityAdminID",
                        column: x => x.UniversityAdminID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingApplications_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingApplications_TrainingOpportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "TrainingOpportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlacements",
                columns: table => new
                {
                    PlacementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityID = table.Column<int>(type: "int", nullable: false),
                    ApplicationID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    InstitutionID = table.Column<int>(type: "int", nullable: false),
                    TermID = table.Column<int>(type: "int", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InstitutionSupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrainingOpportunityOpportunityID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlacements", x => x.PlacementID);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_AspNetUsers_InstitutionSupervisorID",
                        column: x => x.InstitutionSupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_AspNetUsers_UniversitySupervisorID",
                        column: x => x.UniversitySupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_TrainingApplications_ApplicationID",
                        column: x => x.ApplicationID,
                        principalTable: "TrainingApplications",
                        principalColumn: "ApplicationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_TrainingInstitutions_InstitutionID",
                        column: x => x.InstitutionID,
                        principalTable: "TrainingInstitutions",
                        principalColumn: "InstituationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_TrainingOpportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "TrainingOpportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_TrainingOpportunities_TrainingOpportunityOpportunityID",
                        column: x => x.TrainingOpportunityOpportunityID,
                        principalTable: "TrainingOpportunities",
                        principalColumn: "OpportunityID");
                    table.ForeignKey(
                        name: "FK_TrainingPlacements_TrainingTerms_TermID",
                        column: x => x.TermID,
                        principalTable: "TrainingTerms",
                        principalColumn: "TermID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    AttendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlacementID = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InstitutionSupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.AttendanceID);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_AspNetUsers_InstitutionSupervisorID",
                        column: x => x.InstitutionSupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_TrainingPlacements_PlacementID",
                        column: x => x.PlacementID,
                        principalTable: "TrainingPlacements",
                        principalColumn: "PlacementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldVisits",
                columns: table => new
                {
                    VisitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlacementID = table.Column<int>(type: "int", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    MaxScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldVisits", x => x.VisitID);
                    table.ForeignKey(
                        name: "FK_FieldVisits_AspNetUsers_UniversitySupervisorID",
                        column: x => x.UniversitySupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FieldVisits_TrainingPlacements_PlacementID",
                        column: x => x.PlacementID,
                        principalTable: "TrainingPlacements",
                        principalColumn: "PlacementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentEvaluations",
                columns: table => new
                {
                    EvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlacementID = table.Column<int>(type: "int", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InstitutionSupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EvaluationPdfPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEvaluations", x => x.EvaluationID);
                    table.ForeignKey(
                        name: "FK_StudentEvaluations_AspNetUsers_InstitutionSupervisorID",
                        column: x => x.InstitutionSupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentEvaluations_AspNetUsers_UniversitySupervisorID",
                        column: x => x.UniversitySupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentEvaluations_TrainingPlacements_PlacementID",
                        column: x => x.PlacementID,
                        principalTable: "TrainingPlacements",
                        principalColumn: "PlacementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentReports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlacementID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentReports", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_StudentReports_AspNetUsers_UniversitySupervisorID",
                        column: x => x.UniversitySupervisorID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentReports_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentReports_TrainingPlacements_PlacementID",
                        column: x => x.PlacementID,
                        principalTable: "TrainingPlacements",
                        principalColumn: "PlacementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "1", "UniversityTrainingAdmin", "UNIVERSITYTRAININGADMIN" },
                    { "2", "2", "DepartmentHead", "DEPARTMENTHEAD" },
                    { "3", "3", "UniversitySupervisor", "UNIVERSITYSUPERVISOR" },
                    { "4", "4", "InstitutionTrainingOfficer", "INSTITUTIONTRAININGOFFICER" },
                    { "5", "5", "InstitutionSupervisor", "INSTITUTIONSUPERVISOR" },
                    { "6", "6", "Student", "STUDENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleScopes_DepartmentID",
                table: "AspNetRoleScopes",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleScopes_InstitutionID",
                table: "AspNetRoleScopes",
                column: "InstitutionID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleScopes_RoleID",
                table: "AspNetRoleScopes",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleScopes_UniversityID",
                table: "AspNetRoleScopes",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleScopes_UserID",
                table: "AspNetRoleScopes",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_InstitutionSupervisorID",
                table: "AttendanceRecords",
                column: "InstitutionSupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_PlacementID_AttendanceDate",
                table: "AttendanceRecords",
                columns: new[] { "PlacementID", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_UniversityID",
                table: "Departments",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationCodes_UserId",
                table: "EmailVerificationCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_PlacementID",
                table: "FieldVisits",
                column: "PlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_UniversitySupervisorID",
                table: "FieldVisits",
                column: "UniversitySupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityRequestInstitutions_InstitutionID",
                table: "OpportunityRequestInstitutions",
                column: "InstitutionID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityRequestInstitutions_RequestID",
                table: "OpportunityRequestInstitutions",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySkills_OpportunityID_SkillID",
                table: "OpportunitySkills",
                columns: new[] { "OpportunityID", "SkillID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySkills_SkillID",
                table: "OpportunitySkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySpecialties_OpportunityID_SpecialtyID",
                table: "OpportunitySpecialties",
                columns: new[] { "OpportunityID", "SpecialtyID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySpecialties_SpecialtyID",
                table: "OpportunitySpecialties",
                column: "SpecialtyID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_StudentID",
                table: "PortfolioItems",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSkills_RequestID_SkillID",
                table: "RequestSkills",
                columns: new[] { "RequestID", "SkillID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestSkills_SkillID",
                table: "RequestSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSpecialties_RequestID_SpecialtyID",
                table: "RequestSpecialties",
                columns: new[] { "RequestID", "SpecialtyID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestSpecialties_SpecialtyID",
                table: "RequestSpecialties",
                column: "SpecialtyID");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_DepartmentID",
                table: "Specialties",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_Name",
                table: "Specialties",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_InstitutionSupervisorID",
                table: "StudentEvaluations",
                column: "InstitutionSupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_PlacementID",
                table: "StudentEvaluations",
                column: "PlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_UniversitySupervisorID",
                table: "StudentEvaluations",
                column: "UniversitySupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_PlacementID",
                table: "StudentReports",
                column: "PlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_StudentID",
                table: "StudentReports",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_UniversitySupervisorID",
                table: "StudentReports",
                column: "UniversitySupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentID",
                table: "Students",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SpecialtyID",
                table: "Students",
                column: "SpecialtyID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNumber",
                table: "Students",
                column: "StudentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UniversityID",
                table: "Students",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserID",
                table: "Students",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_SkillID",
                table: "StudentSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_StudentID_SkillID",
                table: "StudentSkills",
                columns: new[] { "StudentID", "SkillID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingApplications_DepartmentHeadID",
                table: "TrainingApplications",
                column: "DepartmentHeadID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingApplications_InstitutionOfficerID",
                table: "TrainingApplications",
                column: "InstitutionOfficerID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingApplications_OpportunityID_StudentID",
                table: "TrainingApplications",
                columns: new[] { "OpportunityID", "StudentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingApplications_StudentID",
                table: "TrainingApplications",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingApplications_UniversityAdminID",
                table: "TrainingApplications",
                column: "UniversityAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingInstitutions_UserID",
                table: "TrainingInstitutions",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_InstitutionID",
                table: "TrainingOpportunities",
                column: "InstitutionID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_InstitutionOfficerID",
                table: "TrainingOpportunities",
                column: "InstitutionOfficerID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_RequestID",
                table: "TrainingOpportunities",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_TermID",
                table: "TrainingOpportunities",
                column: "TermID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_TrainingTermTermID",
                table: "TrainingOpportunities",
                column: "TrainingTermTermID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunityRequests_DepartmentHeadID",
                table: "TrainingOpportunityRequests",
                column: "DepartmentHeadID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunityRequests_DepartmentID",
                table: "TrainingOpportunityRequests",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunityRequests_TermID",
                table: "TrainingOpportunityRequests",
                column: "TermID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunityRequests_UniversityAdminID",
                table: "TrainingOpportunityRequests",
                column: "UniversityAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunityRequests_UniversityID",
                table: "TrainingOpportunityRequests",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_ApplicationID",
                table: "TrainingPlacements",
                column: "ApplicationID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_InstitutionID",
                table: "TrainingPlacements",
                column: "InstitutionID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_InstitutionSupervisorID",
                table: "TrainingPlacements",
                column: "InstitutionSupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_OpportunityID",
                table: "TrainingPlacements",
                column: "OpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_StudentID",
                table: "TrainingPlacements",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_TermID",
                table: "TrainingPlacements",
                column: "TermID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_TrainingOpportunityOpportunityID",
                table: "TrainingPlacements",
                column: "TrainingOpportunityOpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_UniversitySupervisorID",
                table: "TrainingPlacements",
                column: "UniversitySupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_UserID",
                table: "Universities",
                column: "UserID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetRoleScopes");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "EmailVerificationCodes");

            migrationBuilder.DropTable(
                name: "FieldVisits");

            migrationBuilder.DropTable(
                name: "OpportunityRequestInstitutions");

            migrationBuilder.DropTable(
                name: "OpportunitySkills");

            migrationBuilder.DropTable(
                name: "OpportunitySpecialties");

            migrationBuilder.DropTable(
                name: "PortfolioItems");

            migrationBuilder.DropTable(
                name: "RequestSkills");

            migrationBuilder.DropTable(
                name: "RequestSpecialties");

            migrationBuilder.DropTable(
                name: "StudentEvaluations");

            migrationBuilder.DropTable(
                name: "StudentReports");

            migrationBuilder.DropTable(
                name: "StudentSkills");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "TrainingPlacements");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "TrainingApplications");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "TrainingOpportunities");

            migrationBuilder.DropTable(
                name: "Specialties");

            migrationBuilder.DropTable(
                name: "TrainingInstitutions");

            migrationBuilder.DropTable(
                name: "TrainingOpportunityRequests");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "TrainingTerms");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
