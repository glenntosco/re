using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Re.Models.RealEstateDB
{
    [Table("Agents")]
    public partial class Agent
    {
        [Required]
        public string Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        [Required]
        public string MobilePhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Specialties { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Picture { get; set; }

        [Required]
        public string Notes { get; set; }
    }
}