using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Data.Models;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class GrapeVariety
    {
        public GrapeVariety()
        {
            this.Products = new HashSet<ProductGrapeVariety>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(GrapeVarietyMaxLength)]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ProductGrapeVariety> Products { get; set; }
    }
}
