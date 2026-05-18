using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace organiza_emprego.Migrations
{
    /// <inheritdoc />
    public partial class RelacionamentoUsuarioCandidatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Candidaturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_UsuarioId",
                table: "Candidaturas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidaturas_Usuarios_UsuarioId",
                table: "Candidaturas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidaturas_Usuarios_UsuarioId",
                table: "Candidaturas");

            migrationBuilder.DropIndex(
                name: "IX_Candidaturas_UsuarioId",
                table: "Candidaturas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Candidaturas");
        }
    }
}
