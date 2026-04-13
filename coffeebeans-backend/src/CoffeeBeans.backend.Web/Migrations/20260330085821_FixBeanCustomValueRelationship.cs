using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeBeans.backend.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixBeanCustomValueRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BeanCustomValues_Beans_BeanId1",
                table: "BeanCustomValues");

            migrationBuilder.DropIndex(
                name: "IX_BeanCustomValues_BeanId1",
                table: "BeanCustomValues");

            migrationBuilder.DropColumn(
                name: "BeanId1",
                table: "BeanCustomValues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BeanId1",
                table: "BeanCustomValues",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BeanCustomValues_BeanId1",
                table: "BeanCustomValues",
                column: "BeanId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BeanCustomValues_Beans_BeanId1",
                table: "BeanCustomValues",
                column: "BeanId1",
                principalTable: "Beans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
