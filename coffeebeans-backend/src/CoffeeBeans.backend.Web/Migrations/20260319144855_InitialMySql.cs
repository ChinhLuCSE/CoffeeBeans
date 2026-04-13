using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeBeans.backend.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Beans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    TastingProfile = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    BagWeightG = table.Column<double>(type: "double", nullable: false),
                    RoastIndex = table.Column<double>(type: "double", nullable: false),
                    NumFarms = table.Column<int>(type: "int", nullable: false),
                    NumAcidityNotes = table.Column<int>(type: "int", nullable: false),
                    NumSweetnessNotes = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<double>(type: "double", nullable: false),
                    Y = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beans", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ColumnName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    DataType = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomColumns", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BeanCustomValues",
                columns: table => new
                {
                    BeanId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CustomColumnId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Value = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    BeanId1 = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeanCustomValues", x => new { x.BeanId, x.CustomColumnId });
                    table.ForeignKey(
                        name: "FK_BeanCustomValues_Beans_BeanId",
                        column: x => x.BeanId,
                        principalTable: "Beans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeanCustomValues_Beans_BeanId1",
                        column: x => x.BeanId1,
                        principalTable: "Beans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeanCustomValues_CustomColumns_CustomColumnId",
                        column: x => x.CustomColumnId,
                        principalTable: "CustomColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BeanCustomValues_BeanId1",
                table: "BeanCustomValues",
                column: "BeanId1");

            migrationBuilder.CreateIndex(
                name: "IX_BeanCustomValues_CustomColumnId",
                table: "BeanCustomValues",
                column: "CustomColumnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeanCustomValues");

            migrationBuilder.DropTable(
                name: "Beans");

            migrationBuilder.DropTable(
                name: "CustomColumns");
        }
    }
}
