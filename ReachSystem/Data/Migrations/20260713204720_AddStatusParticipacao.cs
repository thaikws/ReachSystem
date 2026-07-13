using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReachSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusParticipacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participacoes_UsuarioId",
                table: "Participacoes");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Participacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Participacoes_UsuarioId_EventoId",
                table: "Participacoes",
                columns: new[] { "UsuarioId", "EventoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participacoes_UsuarioId_EventoId",
                table: "Participacoes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Participacoes");

            migrationBuilder.CreateIndex(
                name: "IX_Participacoes_UsuarioId",
                table: "Participacoes",
                column: "UsuarioId");
        }
    }
}
