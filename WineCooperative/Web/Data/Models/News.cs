﻿using System;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class News
    {
        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(NewsMaxLength)]
        public string Name { get; set; }

        [MaxLength(NewsDescriptionMaxLength)]
        public string Description { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string ThemeId { get; set; }

        public Theme Theme { get; set; }
    }
}