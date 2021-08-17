using System.Collections.Generic;

namespace Web.Services.Products.Models
{
    public class ProductSearchPageServiceModel
    {
        public int ProductsPerPage { get; init; }

        public int CurrantPage { get; init; }

        public int TotalProducts { get; set; }

        public IEnumerable<ProductServiceModel> Products { get; set; }
    }
}
