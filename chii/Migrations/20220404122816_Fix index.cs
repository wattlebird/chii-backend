using Microsoft.EntityFrameworkCore.Migrations;

namespace chii.Migrations
{
    public partial class Fixindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_SubjectId_NormalizedContent",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_SubjectEntities_NormalizedAlias",
                table: "SubjectEntities");

            migrationBuilder.DropIndex(
                name: "IX_SubjectEntities_SubjectId",
                table: "SubjectEntities");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SubjectId_NormalizedContent",
                table: "Tags",
                columns: new[] { "SubjectId", "NormalizedContent" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectEntities_SubjectId_NormalizedAlias",
                table: "SubjectEntities",
                columns: new[] { "SubjectId", "NormalizedAlias" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_SubjectId_NormalizedContent",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_SubjectEntities_SubjectId_NormalizedAlias",
                table: "SubjectEntities");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SubjectId_NormalizedContent",
                table: "Tags",
                columns: new[] { "SubjectId", "NormalizedContent" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectEntities_NormalizedAlias",
                table: "SubjectEntities",
                column: "NormalizedAlias");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectEntities_SubjectId",
                table: "SubjectEntities",
                column: "SubjectId");
        }
    }
}
