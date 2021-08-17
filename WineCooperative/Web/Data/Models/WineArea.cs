using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Data.Models
{
    public class WineArea
    {
        public WineArea() => this.Products = new HashSet<Product>();

        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(WineAreaMaxLength)]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
