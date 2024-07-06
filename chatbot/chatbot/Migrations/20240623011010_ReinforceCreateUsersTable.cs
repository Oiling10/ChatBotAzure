using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace chatbot.Migrations
{
    /// <inheritdoc />
    public partial class ReinforceCreateUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreguntasRespuestas",
                columns: table => new
                {
                    PreguntaID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Pregunta = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Respuesta = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreguntasRespuestas", x => x.PreguntaID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "Interacciones",
                columns: table => new
                {
                    InteraccionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioID = table.Column<int>(type: "integer", nullable: false),
                    PreguntaID = table.Column<int>(type: "integer", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Pregunta = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interacciones", x => x.InteraccionID);
                    table.ForeignKey(
                        name: "FK_Interacciones_PreguntasRespuestas_PreguntaID",
                        column: x => x.PreguntaID,
                        principalTable: "PreguntasRespuestas",
                        principalColumn: "PreguntaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interacciones_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    EvaluacionID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InteraccionID = table.Column<int>(type: "integer", nullable: false),
                    Puntuacion = table.Column<int>(type: "integer", nullable: false),
                    Comentarios = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.EvaluacionID);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Interacciones_InteraccionID",
                        column: x => x.InteraccionID,
                        principalTable: "Interacciones",
                        principalColumn: "InteraccionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_InteraccionID",
                table: "Evaluaciones",
                column: "InteraccionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interacciones_PreguntaID",
                table: "Interacciones",
                column: "PreguntaID");

            migrationBuilder.CreateIndex(
                name: "IX_Interacciones_UsuarioID",
                table: "Interacciones",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "Interacciones");

            migrationBuilder.DropTable(
                name: "PreguntasRespuestas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
