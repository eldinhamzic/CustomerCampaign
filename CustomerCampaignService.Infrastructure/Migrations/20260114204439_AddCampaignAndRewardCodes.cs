using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerCampaignService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignAndRewardCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RewardCode",
                table: "RewardEntries",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CampaignCode",
                table: "Campaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RewardEntries_RewardCode",
                table: "RewardEntries",
                column: "RewardCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CampaignCode",
                table: "Campaigns",
                column: "CampaignCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RewardEntries_RewardCode",
                table: "RewardEntries");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_CampaignCode",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "RewardCode",
                table: "RewardEntries");

            migrationBuilder.DropColumn(
                name: "CampaignCode",
                table: "Campaigns");
        }
    }
}
