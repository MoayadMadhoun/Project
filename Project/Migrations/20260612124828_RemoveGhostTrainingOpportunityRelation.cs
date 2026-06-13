using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGhostTrainingOpportunityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlacements_AspNetUsers_InstitutionSupervisorID",
                table: "TrainingPlacements");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlacements_TrainingOpportunities_TrainingOpportunityOpportunityID",
                table: "TrainingPlacements");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPlacements_TrainingOpportunityOpportunityID",
                table: "TrainingPlacements");

            migrationBuilder.DropColumn(
                name: "TrainingOpportunityOpportunityID",
                table: "TrainingPlacements");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlacements_AspNetUsers_InstitutionSupervisorID",
                table: "TrainingPlacements",
                column: "InstitutionSupervisorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlacements_AspNetUsers_InstitutionSupervisorID",
                table: "TrainingPlacements");

            migrationBuilder.AddColumn<int>(
                name: "TrainingOpportunityOpportunityID",
                table: "TrainingPlacements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlacements_TrainingOpportunityOpportunityID",
                table: "TrainingPlacements",
                column: "TrainingOpportunityOpportunityID");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlacements_AspNetUsers_InstitutionSupervisorID",
                table: "TrainingPlacements",
                column: "InstitutionSupervisorID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlacements_TrainingOpportunities_TrainingOpportunityOpportunityID",
                table: "TrainingPlacements",
                column: "TrainingOpportunityOpportunityID",
                principalTable: "TrainingOpportunities",
                principalColumn: "OpportunityID");
        }
    }
}
