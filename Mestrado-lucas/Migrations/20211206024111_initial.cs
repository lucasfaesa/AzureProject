using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mestrado_lucas.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aluno",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoginNome = table.Column<string>(nullable: true),
                    Senha = table.Column<string>(nullable: true),
                    Nome = table.Column<string>(nullable: true),
                    DtCriacao = table.Column<DateTime>(nullable: false),
                    DtUltimoLogin = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aluno", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(nullable: true),
                    Descricao = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sessao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlunoId = table.Column<int>(nullable: true),
                    FaseId = table.Column<int>(nullable: true),
                    Concluida = table.Column<int>(nullable: true),
                    Pontuacao = table.Column<int>(nullable: true),
                    DtConclusao = table.Column<DateTime>(nullable: true),
                    TempoDecorrido = table.Column<float>(nullable: true),
                    DtUltimaJogada = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessao_Aluno_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Aluno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessao_Fase_FaseId",
                        column: x => x.FaseId,
                        principalTable: "Fase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessao_AlunoId",
                table: "Sessao",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessao_FaseId",
                table: "Sessao",
                column: "FaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessao");

            migrationBuilder.DropTable(
                name: "Aluno");

            migrationBuilder.DropTable(
                name: "Fase");
        }
    }
}
