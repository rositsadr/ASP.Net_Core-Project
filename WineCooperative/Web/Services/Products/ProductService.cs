using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Models.Enums;

namespace Web.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly WineCooperativeDbContext data;

        public ProductService(WineCooperativeDbContext data) => this.data = data;

        public ProductSearchPageServiceModel All(string manufacturer, string color, string searchTerm, ProductsSort sorting, int currantPage, int productsPerRage )
        {
            var productsQuery = data.Products.AsQueryable();

            if (!string.IsNullOrEmpty(manufacturer))
            {
                productsQuery = productsQuery
                    .Where(p => p.Manufacturer.Name == manufacturer);
            }

            if (!string.IsNullOrEmpty(color))
            {
                productsQuery = productsQuery
                    .Where(p => p.Color.Name == color);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery
                    .Where(p => (p.Name.ToLower() + " " + p.Description.ToLower()).Contains(searchTerm));
            }

            productsQuery = sorting switch
            {
                ProductsSort.ByYear => productsQuery.OrderByDescending(p => p.ManufactureYear),
                ProductsSort.ByManufacturer => productsQuery.OrderBy(p => p.Manufacturer.Name),
                ProductsSort.ByName or _ => productsQuery.OrderBy(p => p.Name)
            };

            var totalProducts = productsQuery.Count();

            var products = productsQuery
                 .Skip((currantPage - 1) * productsPerRage)
                 .Take(productsPerRage)
                 .Select(p => new ProductServiceModel
                 {
                     Id = p.Id,
                     ImageUrl = p.ImageUrl,
                     Name = p.Name,
                     Price = p.Price,
                     InStock = p.InStock,
                     ManufacturerId = p.ManufacturerId,
                 })
                 .ToList();

            return new ProductSearchPageServiceModel
            {
                TotalProducts = totalProducts,
                CurrantPage = currantPage,
                ProductsPerPage = productsPerRage,
                Products = products,
            };
        }

        public void Delete(string id)
        {
            var product = data.Products
                .Find(id);

            if (product != null)
            {
                data.Remove(product);
                data.SaveChanges();
            }
        }

        public ProductDetailsServiceModel Details(string id) => data.Products
               .Where(p => p.Id == id)
               .Select(p => new ProductDetailsServiceModel
               {
                   Id = p.Id,
                   Name = p.Name,
                   Price = p.Price,
                   ImageUrl = p.ImageUrl,
                   Description = p.Description,
                   InStock = p.InStock,
                   ManufacturerName = p.Manufacturer.Name,
                   ManufactureYear = p.ManufactureYear,
                   WineAreaName = p.WineArea.Name
               })
               .FirstOrDefault();

        public IEnumerable<string> GetAllColors() => data.ProductColors
                .Select(c => c.Name);

        public IEnumerable<string> GetAllManufacturers() => data.Manufacturers
                .Select(m => m.Name);
    }
}
