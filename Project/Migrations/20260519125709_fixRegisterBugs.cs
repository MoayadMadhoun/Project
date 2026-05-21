using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class fixRegisterBugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Universities_UserID",
                table: "Universities");

            migrationBuilder.DropIndex(
                name: "IX_TrainingInstitutions_UserID",
                table: "TrainingInstitutions");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniversityID",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentID",
                table: "Specialties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_UserID",
                table: "Universities",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingInstitutions_UserID",
                table: "TrainingInstitutions",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UniversityID",
                table: "Students",
                column: "UniversityID");

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_DepartmentID",
                table: "Specialties",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Specialties_Departments_DepartmentID",
                table: "Specialties",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Universities_UniversityID",
                table: "Students",
                column: "UniversityID",
                principalTable: "Universities",
                principalColumn: "UniversityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_Departments_DepartmentID",
                table: "Specialties");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Universities_UniversityID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Universities_UserID",
                table: "Universities");

            migrationBuilder.DropIndex(
                name: "IX_TrainingInstitutions_UserID",
                table: "TrainingInstitutions");

            migrationBuilder.DropIndex(
                name: "IX_Students_UniversityID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Specialties_DepartmentID",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UniversityID",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_UserID",
                table: "Universities",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingInstitutions_UserID",
                table: "TrainingInstitutions",
                column: "UserID");
        }
    }
}
