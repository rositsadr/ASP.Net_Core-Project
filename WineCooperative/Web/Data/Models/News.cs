using System;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Data.Models
{
    public class News
    {
        [Key]
        [Required]
        public int Id { get; init; }

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
