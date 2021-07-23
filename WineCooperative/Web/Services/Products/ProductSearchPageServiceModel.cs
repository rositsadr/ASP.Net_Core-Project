using System.Collections.Generic;

namespace Web.Services.Products
{
    public class ProductSearchPageServiceModel
    {
        public int ProductsPerPage { get; init; }

        public int CurrantPage { get; init; }

        public int TotalProducts { get; init; }

        public IEnumerable<ProductServiceModel> Products { get; set; }
    }
}
