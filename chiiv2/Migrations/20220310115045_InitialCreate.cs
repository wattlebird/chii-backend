using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chiiv2.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameCN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Infobox = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Platform = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true),
                    NSFW = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FavCount = table.Column<int>(type: "int", nullable: false),
                    RateCount = table.Column<int>(type: "int", nullable: false),
                    CollectCount = table.Column<int>(type: "int", nullable: false),
                    DoCount = table.Column<int>(type: "int", nullable: false),
                    DroppedCount = table.Column<int>(type: "int", nullable: false),
                    OnHoldCount = table.Column<int>(type: "int", nullable: false),
                    WishCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timestamps",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timestamps", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "CustomRanks",
                columns: table => new
                {
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    SciRank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomRanks", x => x.SubjectId);
                    table.ForeignKey(
                        name: "FK_CustomRanks_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedAlias = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectEntities_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedContent = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserCount = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectEntities_NormalizedAlias",
                table: "SubjectEntities",
                column: "NormalizedAlias");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectEntities_SubjectId",
                table: "SubjectEntities",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SubjectId_NormalizedContent",
                table: "Tags",
                columns: new[] { "SubjectId", "NormalizedContent" },
                unique: true,
                filter: "[NormalizedContent] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomRanks");

            migrationBuilder.DropTable(
                name: "SubjectEntities");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Timestamps");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
