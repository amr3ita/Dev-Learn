using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Code_Road.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinDegree",
                table: "Questions",
                newName: "Degree");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Degree",
                table: "Questions",
                newName: "MinDegree");
        }
    }
}
