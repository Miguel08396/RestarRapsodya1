using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestarRapsodya.Migrations
{
    /// <inheritdoc />
    public partial class primeraMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estado",
                columns: table => new
                {
                    id_Estado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    estado = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estado", x => x.id_Estado);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    id_Rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.id_Rol);
                });

            migrationBuilder.CreateTable(
                name: "Mesa",
                columns: table => new
                {
                    id_Mesa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    id_Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesa", x => x.id_Mesa);
                    table.ForeignKey(
                        name: "FK_Mesa_Estado_id_Estado",
                        column: x => x.id_Estado,
                        principalTable: "Estado",
                        principalColumn: "id_Estado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Correo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    id_Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Correo);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_id_Rol",
                        column: x => x.id_Rol,
                        principalTable: "Rol",
                        principalColumn: "id_Rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    id_Pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha_Pedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_Pedido = table.Column<decimal>(type: "decimal(11,2)", nullable: false),
                    id_Mesa = table.Column<int>(type: "int", nullable: false),
                    mesaid_Mesa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.id_Pedido);
                    table.ForeignKey(
                        name: "FK_Pedido_Mesa_mesaid_Mesa",
                        column: x => x.mesaid_Mesa,
                        principalTable: "Mesa",
                        principalColumn: "id_Mesa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factura",
                columns: table => new
                {
                    id_Factura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha_factura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total = table.Column<decimal>(type: "decimal(11,9)", nullable: false),
                    id_Pedido = table.Column<int>(type: "int", nullable: false),
                    pedidoid_Pedido = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Factura_pedidoid_Pedido",
                table: "Factura",
                column: "pedidoid_Pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_id_Estado",
                table: "Mesa",
                column: "id_Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_mesaid_Mesa",
                table: "Pedido",
                column: "mesaid_Mesa");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_id_Rol",
                table: "Usuario",
                column: "id_Rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Factura");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Mesa");

            migrationBuilder.DropTable(
                name: "Estado");
        }
    }
}
