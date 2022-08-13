using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace chii.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "varchar(80)", nullable: false),
                    NameCN = table.Column<string>(type: "varchar(80)", nullable: true),
                    Infobox = table.Column<string>(type: "json", nullable: true),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    NSFW = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    FavCount = table.Column<int>(type: "integer", nullable: false),
                    RateCount = table.Column<int>(type: "integer", nullable: false),
                    CollectCount = table.Column<int>(type: "integer", nullable: false),
                    DoCount = table.Column<int>(type: "integer", nullable: false),
                    DroppedCount = table.Column<int>(type: "integer", nullable: false),
                    OnHoldCount = table.Column<int>(type: "integer", nullable: false),
                    WishCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timestamps",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timestamps", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "CustomRanks",
                columns: table => new
                {
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    SciRank = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    NormalizedAlias = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    NormalizedContent = table.Column<string>(type: "text", nullable: true),
                    UserCount = table.Column<int>(type: "integer", nullable: false),
                    Confidence = table.Column<double>(type: "double precision", nullable: false)
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
                unique: true);
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
