using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;

namespace Web.Tests.Data
{
    public static class WineAreaTestData
    {
        public static List<WineArea> GetWineAreas(int count) =>
            Enumerable.Range(1, count)
            .Select(i => new WineArea
            {
                Id = i,
                Name = $"Test{i}",
                Products = new List<Product>()
            })
            .ToList();

        public static WineArea GetWineAreaWithProductsAndFullData(int id, int productsCount)
        {
            var wineArea = new WineArea
            {
                Id = id,
                Name = "TestArea",
                Description = "TestDescription",
                Products = new List<Product>(),
            };

            var products = Enumerable
                .Range(1, productsCount)
                .Select(i => new Product
                {
                    Id = i.ToString(),
                    Name = $"TestProduct{i}"
                }
            );

            foreach (var product in products)
            {
                wineArea.Products.Add(product);
            }

            return wineArea;
        }

    }
}
