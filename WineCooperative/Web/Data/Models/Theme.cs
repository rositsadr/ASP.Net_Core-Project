using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Theme
    {
        public Theme() => this.News = new HashSet<News>();

        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(ThemeMaxLength)]
        public string Name { get; set; }

        public ICollection<News> News { get; set; }
    }
}
