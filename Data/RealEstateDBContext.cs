using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Re.Models.RealEstateDB;
namespace Re.Data
{
    public partial class RealEstateDBContext : DbContext
    {
        public RealEstateDBContext()
        {
        }

        public RealEstateDBContext(DbContextOptions<RealEstateDBContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Agent>().HasNoKey();

            builder.Entity<PropertyPicture>()
              .HasOne(i => i.Property)
              .WithMany(i => i.PropertyPictures)
              .HasForeignKey(i => i.PropertyId)
              .HasPrincipalKey(i => i.Id);

            this.OnModelBuilding(builder);
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyPicture> PropertyPictures { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}