using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CharzPiexApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathToDefectEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "DefectEntityItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "DefectEntityItems");
        }
    }
}
