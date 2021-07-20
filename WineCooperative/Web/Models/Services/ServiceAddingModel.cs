﻿using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.Services
{
    public class ServiceAddingModel
    {
        [Required]
        [StringLength(ServiceMaxLength,MinimumLength =ServiceMinLength)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }
    }
}
