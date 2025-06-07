using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wisgenix.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration2203251500 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Topics",
                newName: "TopicName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TopicName",
                table: "Topics",
                newName: "Name");
        }
    }
}
