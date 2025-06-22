using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wisgenix.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUniqueConstraints2206251550 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Topics_TopicName_SubjectID",
                table: "Topics",
                columns: new[] { "TopicName", "SubjectID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectName",
                table: "Subjects",
                column: "SubjectName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionText_TopicID",
                table: "Questions",
                columns: new[] { "QuestionText", "TopicID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topics_TopicName_SubjectID",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SubjectName",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionText_TopicID",
                table: "Questions");
        }
    }
}
