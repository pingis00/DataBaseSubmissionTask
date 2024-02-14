using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRestrictOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
