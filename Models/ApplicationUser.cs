using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Re.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        [JsonIgnore, IgnoreDataMember]
        public override string PasswordHash { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        [JsonIgnore, IgnoreDataMember, NotMapped]
        public string Name
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
            }
        }

        public ICollection<ApplicationRole> Roles { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Position { get; set; }
        public string Specialties { get; set; }
        public string Picture { get; set; }
        public string Notes { get; set; }
    }
}