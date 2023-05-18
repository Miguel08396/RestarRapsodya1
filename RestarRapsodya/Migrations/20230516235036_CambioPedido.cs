using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestarRapsodya.Migrations
{
    /// <inheritdoc />
    public partial class CambioPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Mesa_mesaid_Mesa",
                table: "Pedido");

            migrationBuilder.DropTable(
                name: "Factura");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_mesaid_Mesa",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "total_Pedido",
                table: "Pedido");

            migrationBuilder.RenameColumn(
                name: "mesaid_Mesa",
                table: "Pedido",
                newName: "Cantidad");

            migrationBuilder.AddColumn<string>(
                name: "Correo_Usuario",
                table: "Pedido",
                type: "nvarchar(80)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre_Plato",
                table: "Pedido",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Correo_Usuario",
                table: "Pedido",
                column: "Correo_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_id_Mesa",
                table: "Pedido",
                column: "id_Mesa");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Mesa_id_Mesa",
                table: "Pedido",
                column: "id_Mesa",
                principalTable: "Mesa",
                principalColumn: "id_Mesa",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Usuario_Correo_Usuario",
                table: "Pedido",
                column: "Correo_Usuario",
                principalTable: "Usuario",
                principalColumn: "Correo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Mesa_id_Mesa",
                table: "Pedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Usuario_Correo_Usuario",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_Correo_Usuario",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_id_Mesa",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Correo_Usuario",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Nombre_Plato",
                table: "Pedido");

            migrationBuilder.RenameColumn(
                name: "Cantidad",
                table: "Pedido",
                newName: "mesaid_Mesa");

            migrationBuilder.AddColumn<decimal>(
                name: "total_Pedido",
                table: "Pedido",
                type: "decimal(11,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Factura",
                columns: table => new
                {
                    id_Factura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedidoid_Pedido = table.Column<int>(type: "int", nullable: false),
                    fecha_factura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_Pedido = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<decimal>(type: "decimal(11,9)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factura", x => x.id_Factura);
                    table.ForeignKey(
                        name: "FK_Factura_Pedido_pedidoid_Pedido",
                        column: x => x.pedidoid_Pedido,
                        principalTable: "Pedido",
                        principalColumn: "id_Pedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_mesaid_Mesa",
                table: "Pedido",
                column: "mesaid_Mesa");

            migrationBuilder.CreateIndex(
                name: "IX_Factura_pedidoid_Pedido",
                table: "Factura",
                column: "pedidoid_Pedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Mesa_mesaid_Mesa",
                table: "Pedido",
                column: "mesaid_Mesa",
                principalTable: "Mesa",
                principalColumn: "id_Mesa",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
