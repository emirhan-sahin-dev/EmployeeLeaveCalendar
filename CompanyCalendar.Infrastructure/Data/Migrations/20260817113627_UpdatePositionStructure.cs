using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyCalendar.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePositionStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Positions_Code",
                table: "Positions",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Positions_Code",
                table: "Positions");
        }
    }
}
