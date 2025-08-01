using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Re.Models.RealEstateDB
{
    [Table("Properties")]
    public partial class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Bedrooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public int Garage { get; set; }

        [Required]
        public int SquareMeters { get; set; }

        [Required]
        public int YearBuilt { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Features { get; set; }

        [Required]
        public string NearbyAmenities { get; set; }

        public DateOnly? ClosedDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public ICollection<PropertyPicture> PropertyPictures { get; set; }
    }
}