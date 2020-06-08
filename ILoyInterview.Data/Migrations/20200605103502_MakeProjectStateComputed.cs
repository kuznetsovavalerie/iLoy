using Microsoft.EntityFrameworkCore.Migrations;

namespace ILoyInterview.Data.Migrations
{
    public partial class MakeProjectStateComputed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Projects",
                nullable: false,
                computedColumnSql: "dbo.GetProjectState(Id)",
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Projects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldComputedColumnSql: "dbo.GetProjectState(Id)");
        }
    }
}
