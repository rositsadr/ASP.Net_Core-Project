using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Country
    {
        public Country() => this.Towns = new HashSet<Town>();

        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(CountryMaxLength)]
        public string CountryName { get; init; }

        public ICollection<Town> Towns { get; set; }
    }
}
