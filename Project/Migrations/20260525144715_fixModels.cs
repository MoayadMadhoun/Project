using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class fixModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingOpportunities_OpportunitySpecialties_OpportunitySpecialtyID",
                table: "TrainingOpportunities");

            migrationBuilder.DropIndex(
                name: "IX_TrainingOpportunities_OpportunitySpecialtyID",
                table: "TrainingOpportunities");

            migrationBuilder.DropIndex(
                name: "IX_OpportunitySpecialties_OpportunityID",
                table: "OpportunitySpecialties");

            migrationBuilder.DropColumn(
                name: "OpportunitySpecialtyID",
                table: "TrainingOpportunities");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TrainingInstitutions");

            migrationBuilder.AddColumn<string>(
                name: "InstitutionType",
                table: "TrainingInstitutions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingOpportunityOpportunityID",
                table: "RequestSpecialties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestSpecialties_TrainingOpportunityOpportunityID",
                table: "RequestSpecialties",
                column: "TrainingOpportunityOpportunityID");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestSpecialties_TrainingOpportunities_TrainingOpportunityOpportunityID",
                table: "RequestSpecialties",
                column: "TrainingOpportunityOpportunityID",
                principalTable: "TrainingOpportunities",
                principalColumn: "OpportunityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestSpecialties_TrainingOpportunities_TrainingOpportunityOpportunityID",
                table: "RequestSpecialties");

            migrationBuilder.DropIndex(
                name: "IX_RequestSpecialties_TrainingOpportunityOpportunityID",
                table: "RequestSpecialties");

            migrationBuilder.DropColumn(
                name: "InstitutionType",
                table: "TrainingInstitutions");

            migrationBuilder.DropColumn(
                name: "TrainingOpportunityOpportunityID",
                table: "RequestSpecialties");

            migrationBuilder.AddColumn<int>(
                name: "OpportunitySpecialtyID",
                table: "TrainingOpportunities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TrainingInstitutions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingOpportunities_OpportunitySpecialtyID",
                table: "TrainingOpportunities",
                column: "OpportunitySpecialtyID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySpecialties_OpportunityID",
                table: "OpportunitySpecialties",
                column: "OpportunityID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingOpportunities_OpportunitySpecialties_OpportunitySpecialtyID",
                table: "TrainingOpportunities",
                column: "OpportunitySpecialtyID",
                principalTable: "OpportunitySpecialties",
                principalColumn: "OpportunitySpecialtyID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
