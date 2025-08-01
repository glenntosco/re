using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Re.Data.Migrations.Data.Migrations
{
    [DbContext(typeof(RealEstateDBContext))]
    [Migration("00000000000000_CreateRealEstateSchema")]
    partial class CreateRealEstateSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                 .HasAnnotation("ProductVersion", "3.0.0")
                 .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Re.Models.RealEstateDB.Property", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("Type").ValueGeneratedOnAdd();
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

            modelBuilder.Entity("Re.Models.RealEstateDB.PropertyPicture", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Picture").IsRequired();

                b.Property<string>("PropertyId").IsRequired();

                b.HasKey("Id");

                b.ToTable("Pictures");
            });

            modelBuilder.Entity("Re.Models.RealEstateDB.PropertyPicture", b =>
            {
                b.HasOne("Re.Models.RealEstateDB.Property")
                    .WithMany("Pictures")
                    .HasForeignKey("PropertyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("Re.Models.RealEstateDB.Property", b =>
            {
                b.HasOne("Re.Models.ApplicationUser")
                    .WithMany("Properties")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
