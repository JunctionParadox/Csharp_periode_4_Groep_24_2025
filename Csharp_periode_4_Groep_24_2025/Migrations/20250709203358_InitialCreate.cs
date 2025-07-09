using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Csharp_periode_4_Groep_24_2025.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enclosures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClimateClass = table.Column<short>(type: "smallint", nullable: false),
                    HabitatType = table.Column<short>(type: "smallint", nullable: false),
                    Security = table.Column<short>(type: "smallint", nullable: false),
                    Size = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enclosures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Species = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SizeClass = table.Column<short>(type: "smallint", nullable: false),
                    Diet = table.Column<short>(type: "smallint", nullable: false),
                    Activity = table.Column<short>(type: "smallint", nullable: false),
                    Prey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnclosureId = table.Column<int>(type: "int", nullable: true),
                    SpaceRequirement = table.Column<double>(type: "float", nullable: false),
                    SecurityRequirement = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Animals_Enclosures_EnclosureId",
                        column: x => x.EnclosureId,
                        principalTable: "Enclosures",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Activity", "CategoryId", "Diet", "EnclosureId", "Name", "Prey", "SecurityRequirement", "SizeClass", "SpaceRequirement", "Species" },
                values: new object[,]
                {
                    { 1, (short)1, null, (short)3, null, "Bear", "Fish", (short)2, (short)5, 300.0, "Bear" },
                    { 2, (short)1, null, (short)5, null, "Eagle", "Fish", (short)1, (short)4, 50.0, "Bird" },
                    { 3, (short)3, null, (short)2, null, "Salmon", "None", (short)1, (short)3, 120.0, "Fish" },
                    { 4, (short)1, null, (short)1, null, "Wolf", "Deer", (short)2, (short)5, 300.0, "Catlike" },
                    { 5, (short)2, null, (short)5, null, "Owl", "Fish", (short)1, (short)4, 200.0, "Bird" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Fish" },
                    { 2, "Bird" },
                    { 3, "Mammal" }
                });

            migrationBuilder.InsertData(
                table: "Enclosures",
                columns: new[] { "Id", "ClimateClass", "HabitatType", "Name", "Security", "Size" },
                values: new object[,]
                {
                    { 1, (short)3, (short)2, "Sigma", (short)3, 320.0 },
                    { 2, (short)1, (short)4, "Delta", (short)1, 200.0 },
                    { 3, (short)2, (short)8, "Epsilon", (short)1, 240.0 },
                    { 4, (short)3, (short)8, "Zeta", (short)1, 240.0 },
                    { 5, (short)0, (short)8, "Gamma", (short)1, 320.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_CategoryId",
                table: "Animals",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_EnclosureId",
                table: "Animals",
                column: "EnclosureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Enclosures");
        }
    }
}
