using Web.Models;

namespace Web.Data.Models
{
    public class ProductGrapeVariety
    {
        public string ProductId { get; set; }

        public Product Product { get; set; }

        public int GrapeVarietyId { get; set; }

        public GrapeVariety GrapeVariety { get; set; }
    }
}
