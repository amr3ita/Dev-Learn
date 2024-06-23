using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Code_Road.Migrations
{
    /// <inheritdoc />
    public partial class finishedLessonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_ApplicationUserId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_ApplicationUserId",
                table: "Lessons");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FinishedLessons",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Degree = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinishedLessons", x => new { x.UserId, x.LessonId });
                    table.ForeignKey(
                        name: "FK_FinishedLessons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FinishedLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinishedLessons_LessonId",
                table: "FinishedLessons",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinishedLessons");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ApplicationUserId",
                table: "Lessons",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_ApplicationUserId",
                table: "Lessons",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
