using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using Re.Models;
using Re.Models.RealEstateDB;
namespace Re.Data.Migrations
{
    [DbContext(typeof(RealEstateDBContext))]
    partial class RealEstateModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                 .HasAnnotation("ProductVersion", "3.0.0")
                 .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity(nameof(Property), b =>
            {
                b.Property<int>("Id");
                b.Property<int>("Type").IsRequired();
                b.Property<string>("Title").IsRequired();
                b.Property<string>("Description").IsRequired();
                b.Property<decimal>("Price").IsRequired();
                b.Property<int>("Bedrooms").IsRequired();
                b.Property<int>("Bathrooms").IsRequired();
                b.Property<int>("Garage").IsRequired();
                b.Property<int>("SquareMeters").IsRequired();
                b.Property<int>("YearBuilt").IsRequired();
                b.Property<string>("Address").IsRequired();
                b.Property<string>("Features").IsRequired();
                b.Property<string>("NearbyAmenities").IsRequired();
                b.Property<DateOnly>("ClosedDate");

                b.Property<string>("UserId").IsRequired();

                b.HasKey("Id");

                b.ToTable("Properties");
            });

            modelBuilder.Entity(nameof(PropertyPicture), b =>
            {
                b.Property<int>("Id").UseIdentityColumn();
                b.Property<string>("Picture").IsRequired();

                b.Property<string>("PropertyId").IsRequired();

                b.HasKey("Id");

                b.ToTable("Pictures");
            });

            modelBuilder.Entity(nameof(PropertyPicture), b =>
            {
                b.HasOne(nameof(Property))
                    .WithMany("Pictures")
                    .HasForeignKey("PropertyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity(nameof(Property), b =>
            {
                b.HasOne(nameof(ApplicationUser))
                    .WithMany("Properties")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
