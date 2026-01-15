using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerCampaignService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseTransactions",
                columns: table => new
                {
                    PurchaseTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseTransactions", x => x.PurchaseTransactionId);
                    table.ForeignKey(
                        name: "FK_PurchaseTransactions_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseTransactions_PurchaseImports_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "PurchaseImports",
                        principalColumn: "ImportBatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_CampaignId_CustomerId",
                table: "PurchaseTransactions",
                columns: new[] { "CampaignId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_CustomerId",
                table: "PurchaseTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_ImportBatchId",
                table: "PurchaseTransactions",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseTransactions_TransactionId",
                table: "PurchaseTransactions",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseTransactions");
        }
    }
}
