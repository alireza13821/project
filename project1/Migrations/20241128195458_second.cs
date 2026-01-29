using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project1.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reserveItems_Books_BookId",
                table: "reserveItems");

            migrationBuilder.DropForeignKey(
                name: "FK_reserveItems_Reserves_ReserveId",
                table: "reserveItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reserveItems",
                table: "reserveItems");

            migrationBuilder.RenameTable(
                name: "reserveItems",
                newName: "ReserveItems");

            migrationBuilder.RenameIndex(
                name: "IX_reserveItems_ReserveId",
                table: "ReserveItems",
                newName: "IX_ReserveItems_ReserveId");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Reserves",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReserveItems",
                table: "ReserveItems",
                columns: new[] { "BookId", "ReserveId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveItems_Books_BookId",
                table: "ReserveItems",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReserveItems_Reserves_ReserveId",
                table: "ReserveItems",
                column: "ReserveId",
                principalTable: "Reserves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReserveItems_Books_BookId",
                table: "ReserveItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReserveItems_Reserves_ReserveId",
                table: "ReserveItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReserveItems",
                table: "ReserveItems");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Reserves");

            migrationBuilder.RenameTable(
                name: "ReserveItems",
                newName: "reserveItems");

            migrationBuilder.RenameIndex(
                name: "IX_ReserveItems_ReserveId",
                table: "reserveItems",
                newName: "IX_reserveItems_ReserveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reserveItems",
                table: "reserveItems",
                columns: new[] { "BookId", "ReserveId" });

            migrationBuilder.AddForeignKey(
                name: "FK_reserveItems_Books_BookId",
                table: "reserveItems",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reserveItems_Reserves_ReserveId",
                table: "reserveItems",
                column: "ReserveId",
                principalTable: "Reserves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
