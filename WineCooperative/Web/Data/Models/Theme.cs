using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Data.Models
{
    public class Theme
    {
        public Theme() => this.News = new HashSet<News>();

        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(ThemeMaxLength)]
        public string Name { get; set; }

        public ICollection<News> News { get; set; }
    }
}
