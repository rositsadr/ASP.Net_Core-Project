﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Town
    {
        public Town()
        {
            this.Addresses = new HashSet<Address>();
        }

        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(TownMaxLength)]
        public string Name { get; set; }

        public string WineAreaId { get; set; }

        public WineArea WineArea { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
