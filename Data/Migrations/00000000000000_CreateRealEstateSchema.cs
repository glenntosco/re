using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Re.Data.Migrations.Data.Migrations
{
    public partial class CreateRealEstateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Bedrooms = table.Column<int>(nullable: false),
                    Bathrooms = table.Column<int>(nullable: false),
                    Garage = table.Column<int>(nullable: false),
                    SquareMeters = table.Column<int>(nullable: false),
                    YearBuilt = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    Features = table.Column<string>(nullable: false),
                    NearbyAmenities = table.Column<string>(nullable: false),
                    ClosedDate = table.Column<DateOnly>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });
          
            migrationBuilder.CreateTable(
                name: "PropertyPictures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Picture = table.Column<string>(nullable: false),
                    PropertyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyPictures_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                CREATE VIEW Agents AS 
                SELECT Id, Email, PhoneNumber, UserName, MobilePhoneNumber, FirstName, LastName, Specialties, Position, Picture, Notes 
                FROM AspNetUsers;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "PropertyPictures");

            migrationBuilder.Sql("DROP VIEW IF EXISTS Agents");
        }
    }
}
