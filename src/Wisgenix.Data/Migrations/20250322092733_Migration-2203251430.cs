using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wisgenix.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration2203251430 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObtainedScore",
                table: "UserResponses");

            migrationBuilder.AddColumn<int>(
                name: "ObtainedScore",
                table: "UserAssessmentQuestions",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObtainedScore",
                table: "UserAssessmentQuestions");

            migrationBuilder.AddColumn<int>(
                name: "ObtainedScore",
                table: "UserResponses",
                type: "INTEGER",
                nullable: true);
        }
    }
}
