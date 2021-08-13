using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;
using Web.Models;

namespace Web.Tests.Data
{
    public static class ProductTestData
    {      
        public static List<Product> GetProducts(string manufacturerId, string userId, int count, bool inStock = true)
        {
            var manufacturer = new Manufacturer()
            {
                Id = manufacturerId,
                UserId = userId,
                Name = $"Manufacturer{manufacturerId}",
            };

            var products = Enumerable
                .Range(1, count)
                .Select(i => new Product
                {
                    Id = i.ToString(),
                    Name = $"Product {i}",
                    ImageUrl = $"www.productImage{i}",
                    InStock = inStock,
                    Price = i,
                    ManufacturerId =manufacturerId, 
                    Manufacturer = manufacturer
                })
                .ToList();

            return products;
        }

        public static List<CartItem> GetCartItems(string manufacturerId, string ownerUserId, int count, string buyerUserId, int quantity)
        {
            var products = GetProducts(manufacturerId, ownerUserId, count);

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
