using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PrecisionFullCoilHMI.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumberOfJobs = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobNumber = table.Column<short>(type: "smallint", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false),
                    SideA = table.Column<short>(type: "smallint", nullable: false),
                    SideB = table.Column<short>(type: "smallint", nullable: false),
                    DuctType = table.Column<short>(type: "smallint", nullable: false),
                    Lock = table.Column<short>(type: "smallint", nullable: false),
                    Connector = table.Column<short>(type: "smallint", nullable: false),
                    Cleats = table.Column<short>(type: "smallint", nullable: false),
                    CleatEdges = table.Column<short>(type: "smallint", nullable: false),
                    SideAHoles = table.Column<short>(type: "smallint", nullable: false),
                    SideBHoles = table.Column<short>(type: "smallint", nullable: false),
                    HoleDie = table.Column<short>(type: "smallint", nullable: false),
                    HoleSize = table.Column<short>(type: "smallint", nullable: false),
                    Bead = table.Column<short>(type: "smallint", nullable: false),
                    Insulation = table.Column<short>(type: "smallint", nullable: false),
                    PinSpacing = table.Column<short>(type: "smallint", nullable: false),
                    Sealant = table.Column<short>(type: "smallint", nullable: false),
                    Gauge = table.Column<short>(type: "smallint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecipeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_RecipeId",
                table: "Jobs",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
