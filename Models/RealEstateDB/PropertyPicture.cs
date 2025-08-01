using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Re.Models.RealEstateDB
{
    [Table("PropertyPictures")]
    public partial class PropertyPicture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Picture { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public Property Property { get; set; }
    }
}