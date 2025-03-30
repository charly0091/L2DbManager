using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDbManager.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Sellable = table.Column<bool>(type: "bit", nullable: false),
                    Tradeable = table.Column<bool>(type: "bit", nullable: false),
                    Dropable = table.Column<bool>(type: "bit", nullable: false),
                    Destroyable = table.Column<bool>(type: "bit", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Accessory_BodyPart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accessory_Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accessory_Crystallizable = table.Column<bool>(type: "bit", nullable: true),
                    ArmorType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyPart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Crystallizable = table.Column<bool>(type: "bit", nullable: true),
                    Stackable = table.Column<bool>(type: "bit", nullable: true),
                    Jewelry_BodyPart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jewelry_Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jewelry_Crystallizable = table.Column<bool>(type: "bit", nullable: true),
                    WeaponType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weapon_BodyPart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weapon_Grade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weapon_Crystallizable = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    ArmorId = table.Column<int>(type: "int", nullable: true),
                    JewelryId = table.Column<int>(type: "int", nullable: true),
                    WeaponId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stats_Items_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stats_Items_JewelryId",
                        column: x => x.JewelryId,
                        principalTable: "Items",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stats_Items_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stats_ArmorId",
                table: "Stats",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_JewelryId",
                table: "Stats",
                column: "JewelryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_WeaponId",
                table: "Stats",
                column: "WeaponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
