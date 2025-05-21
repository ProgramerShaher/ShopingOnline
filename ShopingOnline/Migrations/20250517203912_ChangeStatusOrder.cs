using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopingOnline.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "OrderStatusNew");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderStatusNew",
                table: "Orders",
                newName: "Status");
        }
    }
}
