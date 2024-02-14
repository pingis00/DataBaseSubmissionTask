using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerReviewRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPreferences_PreferredContactMethod",
                table: "ContactPreferences",
                column: "PreferredContactMethod",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews");

            migrationBuilder.DropIndex(
                name: "IX_ContactPreferences_PreferredContactMethod",
                table: "ContactPreferences");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersReviews_Customers_CustomerId",
                table: "CustomersReviews",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
