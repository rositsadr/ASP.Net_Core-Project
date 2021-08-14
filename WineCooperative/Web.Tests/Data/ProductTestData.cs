using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;
using Web.Models;
using static Web.Tests.Data.ManufacturerTestData;


namespace Web.Tests.Data
{
    public static class ProductTestData
    {
        public static GrapeVariety TestGrape(int grapeId) =>
             new GrapeVariety
            {
                Id = grapeId,
                Name = "TestGrape",
            };

        public static WineArea TestWineArea(int wineAreaId) =>
            new WineArea
            {
                Id = wineAreaId,
                Name = "TestWineArea",
            };

        public static ProductTaste TestTaste (int tasteId) =>
            new ProductTaste
            {
                Id = tasteId,
                Name = "TestTaste",
            };

        public static ProductColor TestColor (int colorId) =>
            new ProductColor
            {
                Id = colorId,
                Name = "TestColor",
            };

        public static List<Product> GetProducts(string manufacturerId, string userId, int count, int tasteId, int wineAreaId, int colorId, bool inStock = true)
        {
            var manufacturer = ManufacturerWithUser(userId, manufacturerId);
            var taste = TestTaste(tasteId);
            var color = TestColor(colorId);
            var wineArea = TestWineArea(wineAreaId);

            var products = Enumerable
                .Range(1, count)
                .Select(i => new Product
                {
                    Id = i.ToString(),
                    Name = $"Product {i}",
                    ImageUrl = $"https://images.vivino.com/thumbs/i8zMkR-wQniSPG1QDvm8Ow_pb_600x60{i}.png",
                    InStock = inStock,
                    Price = i,
                    ManufactureYear = 1950 + i,
                    ManufacturerId = manufacturerId,
                    Manufacturer = manufacturer,
                    Color=color,
                    ColorId = color.Id,
                    Taste = taste,
                    TasteId = taste.Id,
                    WineArea = wineArea,
                    WineAreaId =wineArea.Id,
                    GrapeVarieties = new List<ProductGrapeVariety>()
                    {
                        new ProductGrapeVariety() { GrapeVarietyId = i}
                    }
                })
                .ToList();

            return products;
        }

        public static List<CartItem> GetCartItems(string manufacturerId, string ownerUserId, int count, string buyerUserId, int quantity, int tasteId, int colorId, int wineAreId)
        {
            var products = GetProducts(manufacturerId, ownerUserId, count, tasteId, wineAreId, colorId);

            var cartItems = Enumerable
                 .Range(1, count)
                 .Select(i => new CartItem
                 {
                     ProductId = i.ToString(),
                     UserId = buyerUserId,
                     Quantity = quantity,
                     Product = products.Where(p=>p.Id == i.ToString()).FirstOrDefault()
                 })
                 .ToList();

            return cartItems;
        }

        public static string ProductId =>
                new Guid().ToString();
    }
}
