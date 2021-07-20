using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ProductTaste
    {
        public ProductTaste() => this.Products = new HashSet<Product>();

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}