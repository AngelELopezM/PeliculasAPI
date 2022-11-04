using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasAPI.Migrations
{
    public partial class TablasSalaDeCine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Generos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PeliculasActores",
                keyColumns: new[] { "ActorId", "PeliculaId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "PeliculasActores",
                keyColumns: new[] { "ActorId", "PeliculaId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "PeliculasActores",
                keyColumns: new[] { "ActorId", "PeliculaId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "PeliculasActores",
                keyColumns: new[] { "ActorId", "PeliculaId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "PeliculasActores",
                keyColumns: new[] { "ActorId", "PeliculaId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "PeliculasGeneros",
                keyColumns: new[] { "GeneroId", "PeliculaId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "Actores",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Actores",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Actores",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Generos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Generos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Generos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Peliculas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Peliculas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Peliculas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Peliculas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Peliculas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.CreateTable(
                name: "SalasDeCine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalasDeCine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeliculasSalasDeCines",
                columns: table => new
                {
                    PeliculaId = table.Column<int>(type: "int", nullable: false),
                    SalaDeCineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculasSalasDeCines", x => new { x.PeliculaId, x.SalaDeCineId });
                    table.ForeignKey(
                        name: "FK_PeliculasSalasDeCines_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculasSalasDeCines_SalasDeCine_SalaDeCineId",
                        column: x => x.SalaDeCineId,
                        principalTable: "SalasDeCine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasSalasDeCines_SalaDeCineId",
                table: "PeliculasSalasDeCines",
                column: "SalaDeCineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeliculasSalasDeCines");

            migrationBuilder.DropTable(
                name: "SalasDeCine");

            migrationBuilder.InsertData(
                table: "Actores",
                columns: new[] { "Id", "FechaNacimiento", "Foto", "Nombre" },
                values: new object[,]
                {
                    { 5, new DateTime(1962, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Jim Carrey" },
                    { 6, new DateTime(1965, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Robert Downey Jr." },
                    { 7, new DateTime(1981, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Chris Evans" }
                });

            migrationBuilder.InsertData(
                table: "Generos",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 4, "Aventura" },
                    { 5, "Animación" },
                    { 6, "Suspenso" },
                    { 7, "Romance" }
                });

            migrationBuilder.InsertData(
                table: "Peliculas",
                columns: new[] { "Id", "EnCines", "FechaEstreno", "Poster", "Titulo" },
                values: new object[,]
                {
                    { 2, true, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers: Endgame" },
                    { 3, false, new DateTime(2019, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Avengers: Infinity Wars" },
                    { 4, false, new DateTime(2020, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Sonic the Hedgehog" },
                    { 5, false, new DateTime(2020, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Emma" },
                    { 6, false, new DateTime(2020, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Wonder Woman 1984" }
                });

            migrationBuilder.InsertData(
                table: "PeliculasActores",
                columns: new[] { "ActorId", "PeliculaId", "Orden", "Personaje" },
                values: new object[,]
                {
                    { 5, 4, 1, "Dr. Ivo Robotnik" },
                    { 6, 2, 1, "Tony Stark" },
                    { 6, 3, 1, "Tony Stark" },
                    { 7, 2, 2, "Steve Rogers" },
                    { 7, 3, 2, "Steve Rogers" }
                });

            migrationBuilder.InsertData(
                table: "PeliculasGeneros",
                columns: new[] { "GeneroId", "PeliculaId" },
                values: new object[,]
                {
                    { 4, 2 },
                    { 4, 3 },
                    { 4, 4 },
                    { 4, 6 },
                    { 6, 2 },
                    { 6, 3 },
                    { 6, 5 },
                    { 6, 6 },
                    { 7, 5 }
                });
        }
    }
}
