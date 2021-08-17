using System.Collections.Generic;
using Web.ViewModels.Products.Enums;
using Web.Services.Products.Models;

namespace Web.ViewModels.Products
{
    public class ProductSearchPageModel
    {
        public const int productsPerPage = 3;

        public int CurrantPage { get; init; } = 1;

        public int TotalProducts { get; set; }

        public string Color { get; init; }

        public IEnumerable<string> Colors { get; set; }

        public string SearchTerm { get; init; }

        public ProductsSort Sorting  { get; init; }

        public IEnumerable<ProductServiceModel> Products { get; set; }
    }
}
