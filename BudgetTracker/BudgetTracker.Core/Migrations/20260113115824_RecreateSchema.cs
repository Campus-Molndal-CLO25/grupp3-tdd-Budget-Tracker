using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Core.Migrations
{
    /// <inheritdoc />
    public partial class RecreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Transactions",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Categories",
                type: "TEXT",
                maxLength: 24,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Transactions",
                newName: "Note");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
