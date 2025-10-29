using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUserAndFixModelCasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "orderDate",
                table: "orderHeaders",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "SessionID",
                table: "orderHeaders",
                newName: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "orderHeaders",
                newName: "SessionID");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "orderHeaders",
                newName: "orderDate");
        }
    }
}
