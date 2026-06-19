using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReachSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFotoAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Animais",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Animais");
        }
    }
}
