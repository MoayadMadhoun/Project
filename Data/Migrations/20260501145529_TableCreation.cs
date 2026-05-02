using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Data.Migrations
{
    /// <inheritdoc />
    public partial class TableCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    SpecialtyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.SpecialtyID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingInstitutions",
                columns: table => new
                {
                    InstituationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPersonName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingInstitutions", x => x.InstituationID);
                });

            migrationBuilder.CreateTable(
                name: "TrainingTerms",
                columns: table => new
                {
                    TermID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingTerms", x => x.TermID);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    UniversityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.UniversityID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        principalColumn: "UniversityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleScopes",
                columns: table => new
                {
                    UserRoleScopeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UniversityID = table.Column<int>(type: "int", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    InstitutionID = table.Column<int>(type: "int", nullable: false),
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
                name: "Students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GPA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CVPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    SpecialtyID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Specialties_SpecialtyID",
                        column: x => x.SpecialtyID,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingOpportunityRequests",
                columns: table => new
                {
                    RequestID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedSeats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreferredEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: true),
                    UniversityID = table.Column<int>(type: "int", nullable: false),
                    TermID = table.Column<int>(type: "int", nullable: false),
                    TrainingTermTermID = table.Column<int>(type: "int", nullable: false),
                    UniversityAdminID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepartmentHeadID = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                        name: "FK_TrainingOpportunityRequests_TrainingTerms_TrainingTermTermID",
                        column: x => x.TrainingTermTermID,
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
                name: "PortfolioItems",
                columns: table => new
                {
                    PortfolioItemID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    StudentSkillID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "OpportunityRequestInstitutions",
                columns: table => new
                {
                    RequestInstitutionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    RequestID1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstitutionID = table.Column<int>(type: "int", nullable: false),
                    TrainingInstitutionInstituationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityRequestInstitutions", x => x.RequestInstitutionID);
                    table.ForeignKey(
                        name: "FK_OpportunityRequestInstitutions_TrainingInstitutions_TrainingInstitutionInstituationID",
                        column: x => x.TrainingInstitutionInstituationID,
                        principalTable: "TrainingInstitutions",
                        principalColumn: "InstituationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpportunityRequestInstitutions_TrainingOpportunityRequests_RequestID1",
                        column: x => x.RequestID1,
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
                    RequestID1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_RequestSkills_TrainingOpportunityRequests_RequestID1",
                        column: x => x.RequestID1,
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
                    RequestID1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_RequestSpecialties_TrainingOpportunityRequests_RequestID1",
                        column: x => x.RequestID1,
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
                    TrainingInstitutionInstituationID = table.Column<int>(type: "int", nullable: false),
                    TermID = table.Column<int>(type: "int", nullable: false),
                    TrainingTermTermID = table.Column<int>(type: "int", nullable: false),
                    RequestID = table.Column<int>(type: "int", nullable: true),
                    RequestID1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstitutionOfficerID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        name: "FK_TrainingOpportunities_TrainingInstitutions_TrainingInstitutionInstituationID",
                        column: x => x.TrainingInstitutionInstituationID,
                        principalTable: "TrainingInstitutions",
                        principalColumn: "InstituationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_TrainingOpportunityRequests_RequestID1",
                        column: x => x.RequestID1,
                        principalTable: "TrainingOpportunityRequests",
                        principalColumn: "RequestID");
                    table.ForeignKey(
                        name: "FK_TrainingOpportunities_TrainingTerms_TrainingTermTermID",
                        column: x => x.TrainingTermTermID,
                        principalTable: "TrainingTerms",
                        principalColumn: "TermID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpportunitySkills",
                columns: table => new
                {
                    OpportunitySkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityID = table.Column<int>(type: "int", nullable: false),
                    TrainingOpportunityOpportunityID = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_OpportunitySkills_TrainingOpportunities_TrainingOpportunityOpportunityID",
                        column: x => x.TrainingOpportunityOpportunityID,
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
                    TrainingOpportunityOpportunityID = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_OpportunitySpecialties_TrainingOpportunities_TrainingOpportunityOpportunityID",
                        column: x => x.TrainingOpportunityOpportunityID,
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
                    TrainingOpportunityOpportunityID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentDecision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentHeadID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DepartmentReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentReviewNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityAdminDecision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityAdminID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UniversityAdminReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UniversityAdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstitutionDecision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstitutionOfficerID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InstitutionReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InstitutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingApplications_TrainingOpportunities_TrainingOpportunityOpportunityID",
                        column: x => x.TrainingOpportunityOpportunityID,
                        principalTable: "TrainingOpportunities",
                        principalColumn: "OpportunityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlacements",
                columns: table => new
                {
                    PlacementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityID = table.Column<int>(type: "int", nullable: false),
                    ApplicationID = table.Column<int>(type: "int", nullable: false),
                    TrainingApplicationApplicationID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    InstitutionID = table.Column<int>(type: "int", nullable: false),
                    TermID = table.Column<int>(type: "int", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstitutionSupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_TrainingPlacements_TrainingApplications_TrainingApplicationApplicationID",
                        column: x => x.TrainingApplicationApplicationID,
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
                    TrainingPlacementPlacementID = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstitutionSupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_AttendanceRecords_TrainingPlacements_TrainingPlacementPlacementID",
                        column: x => x.TrainingPlacementPlacementID,
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
                    TrainingPlacementPlacementID = table.Column<int>(type: "int", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        name: "FK_FieldVisits_TrainingPlacements_TrainingPlacementPlacementID",
                        column: x => x.TrainingPlacementPlacementID,
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
                    TrainingPlacementPlacementID = table.Column<int>(type: "int", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InstitutionSupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EvaluationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluationPdfPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        name: "FK_StudentEvaluations_TrainingPlacements_TrainingPlacementPlacementID",
                        column: x => x.TrainingPlacementPlacementID,
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
                    TrainingPlacementPlacementID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversitySupervisorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_StudentReports_TrainingPlacements_TrainingPlacementPlacementID",
                        column: x => x.TrainingPlacementPlacementID,
                        principalTable: "TrainingPlacements",
                        principalColumn: "PlacementID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "UniversityTrainingAdmin", "UNIVERSITYTRAININGADMIN" },
                    { "2", null, "DepartmentHead", "DEPARTMENTHEAD" },
                    { "3", null, "UniversitySupervisor", "UNIVERSITYSUPERVISOR" },
                    { "4", null, "InstitutionTrainingOfficer", "INSTITUTIONTRAININGOFFICER" },
                    { "5", null, "InstitutionSupervisor", "INSTITUTIONSUPERVISOR" },
                    { "6", null, "Student", "STUDENT" }
                });

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
                column: "UserID");

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
                name: "IX_AttendanceRecords_TrainingPlacementPlacementID",
                table: "AttendanceRecords",
                column: "TrainingPlacementPlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_UniversityID",
                table: "Departments",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_TrainingPlacementPlacementID",
                table: "FieldVisits",
                column: "TrainingPlacementPlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_FieldVisits_UniversitySupervisorID",
                table: "FieldVisits",
                column: "UniversitySupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityRequestInstitutions_RequestID1",
                table: "OpportunityRequestInstitutions",
                column: "RequestID1");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityRequestInstitutions_TrainingInstitutionInstituationID",
                table: "OpportunityRequestInstitutions",
                column: "TrainingInstitutionInstituationID");

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
                name: "IX_OpportunitySkills_TrainingOpportunityOpportunityID",
                table: "OpportunitySkills",
                column: "TrainingOpportunityOpportunityID");

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
                name: "IX_OpportunitySpecialties_TrainingOpportunityOpportunityID",
                table: "OpportunitySpecialties",
                column: "TrainingOpportunityOpportunityID");

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
                name: "IX_RequestSkills_RequestID1",
                table: "RequestSkills",
                column: "RequestID1");

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
                name: "IX_RequestSpecialties_RequestID1",
                table: "RequestSpecialties",
                column: "RequestID1");

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
                name: "IX_Specialties_Name",
                table: "Specialties",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_InstitutionSupervisorID",
                table: "StudentEvaluations",
                column: "InstitutionSupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_TrainingPlacementPlacementID",
                table: "StudentEvaluations",
                column: "TrainingPlacementPlacementID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluations_UniversitySupervisorID",
                table: "StudentEvaluations",
                column: "UniversitySupervisorID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_StudentID",
                table: "StudentReports",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentReports_TrainingPlacementPlacementID",
                table: "StudentReports",
                column: "TrainingPlacementPlacementID");

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
                name: "IX_TrainingApplications_TrainingOpportunityOpportunityID",
                table: "TrainingApplications",
                column: "TrainingOpportunityOpportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingApplications_UniversityAdminID",
                table: "TrainingApplications",
                column: "UniversityAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_InstitutionOfficerID",
                table: "TrainingOpportunities",
                column: "InstitutionOfficerID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_RequestID1",
                table: "TrainingOpportunities",
                column: "RequestID1");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_TrainingInstitutionInstituationID",
                table: "TrainingOpportunities",
                column: "TrainingInstitutionInstituationID");

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
                name: "IX_TrainingOpportunityRequests_TrainingTermTermID",
                table: "TrainingOpportunityRequests",
                column: "TrainingTermTermID");

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
                name: "IX_TrainingPlacements_TrainingApplicationApplicationID",
                table: "TrainingPlacements",
                column: "TrainingApplicationApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_UniversitySupervisorID",
                table: "TrainingPlacements",
                column: "UniversitySupervisorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleScopes");

            migrationBuilder.DropTable(
                name: "AttendanceRecords");

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

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");
        }
    }
}
