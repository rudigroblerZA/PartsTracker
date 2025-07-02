using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "parts",
                schema: "public",
                columns: table => new
                {
                    partnumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    quantityonhand = table.Column<int>(type: "integer", nullable: false),
                    locationcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    laststocktake = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parts", x => x.partnumber);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parts",
                schema: "public");
        }
    }
}
