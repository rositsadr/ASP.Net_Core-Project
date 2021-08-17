using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Data.Models
{
    public class Country
    {
        public Country() => this.Towns = new HashSet<Town>();

        public int Id { get; set; }

        [Required]
        [MaxLength(CountryMaxLength)]
        public string CountryName { get; set; }

        public ICollection<Town> Towns { get; set; }
    }
}
