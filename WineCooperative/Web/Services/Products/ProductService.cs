using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Models;
using Web.Models.Enums;
using Web.Services.Products.Models;

namespace Web.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly WineCooperativeDbContext data;

        public ProductService(WineCooperativeDbContext data) => this.data = data;

        public string CreateProduct(string name, decimal price, string imageUrl, int manufactureYear, string description, bool inStock, int wineAreaId, string manufacturerId, int tasteId, int colorId, IEnumerable<int> grapeVarieties)
        {
            var productToImport = new Product
            {
                Name = name,
                Price = price,
                ImageUrl = imageUrl,
                ManufactureYear = manufactureYear,
                Description = description,
                InStock = inStock,
                WineAreaId = wineAreaId,
                GrapeVarieties = new List<ProductGrapeVariety>(),
                ManufacturerId = manufacturerId,
                TasteId = tasteId,
                ColorId = colorId
            };

            foreach (var grapeId in grapeVarieties)
            {
                productToImport.GrapeVarieties.Add(new ProductGrapeVariety
                {
                    ProductId = productToImport.Id,
                    GrapeVarietyId = grapeId
                });
            }

            data.Products.Add(productToImport);

            data.SaveChanges();

            return productToImport.Id;
        }

        public ProductSearchPageServiceModel All(string color, string searchTerm, ProductsSort sorting, int currantPage, int productsPerRage )
        {
            var productsQuery = data.Products
                .Where(p=>p.InStock == true)
                .AsQueryable();

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

        public IEnumerable<ProductDetailsServiceModel> ProductsByUser(string userId) => this.GetProducts(data.Products
            .Where(p => p.Manufacturer.UserId == userId));

        public ProductEditServiceModel Edit(string id)
        {
            return data.Products
                 .Where(p => p.Id == id)
                 .Select(p => new ProductEditServiceModel
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Price = p.Price,
                     ManufacturerId = p.ManufacturerId,
                     ManufactureYear = p.ManufactureYear,
                     Description = p.Description,
                     InStock = p .InStock,
                     ImageUrl = p.ImageUrl,
                     ColorId = p.ColorId,
                     TasteId = p.TasteId,
                     WineAreaId = p.WineAreaId,
                     UserId = p.Manufacturer.UserId,
                     GrapeVarieties = p.GrapeVarieties
                                        .Select(gv=>gv.GrapeVarietyId),                     
                 })
                 .FirstOrDefault();
        }

        public ProductDetailsServiceModel Details(string id) => this.GetProducts(data.Products
            .Where(p => p.Id == id))
            .FirstOrDefault();

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

        public IEnumerable<string> GetAllColorsName() => data.ProductColors
                .Select(c => c.Name);

        public IEnumerable<ProductWineAreaServiceModel> GetAllWineAreas() => this.data.WineAreas
          .Select(wa => new ProductWineAreaServiceModel
          {
              WineAreaId = wa.Id,
              WineAreName = wa.Name
          });

        public IEnumerable<ProductGrapeVarietiesServiceModel> GetAllGrapeVarieties() => this.data.GrapeVarieties
            .Select(gv => new ProductGrapeVarietiesServiceModel
            {
                GrapeVarietyId = gv.Id,
                GrapeVarietyName = gv.Name
            });

        public IEnumerable<ProductManufacturerServiceModel> GetAllManufacturers()
        {
            var manufacturers = this.data.Manufacturers
                .Select(m => new ProductManufacturerServiceModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    UserId = m.UserId                     
                })
                .ToList();

            return manufacturers;
        }

        public IEnumerable<ProductColorServiceModel> GetAllColors() => this.data.ProductColors
            .Select(m => new ProductColorServiceModel
            {
                Id = m.Id,
                Name = m.Name
            });

        public IEnumerable<ProductTasteServiceModel> GetAllTastes() => this.data.ProductTastes
            .Select(m => new ProductTasteServiceModel
            {
                Id = m.Id,
                Name = m.Name
            });

        public bool ColorExists(int colorId) => this.data.ProductColors
            .Any(pc => pc.Id == colorId);

        public bool TasteExists(int tasteId) => this.data.ProductTastes
            .Any(pt => pt.Id == tasteId);

        public bool WineAreaExists(int wineAreaId) => this.data.WineAreas
            .Any(w => w.Id == wineAreaId);

        public bool ManufacturerExists(string manufacturerId) => this.data.Manufacturers
            .Any(m => m.Id == manufacturerId);

        public bool GrapeVarietiesExists(IEnumerable<int> grapeVarieties)
        {
            foreach (var grapeId in grapeVarieties)
            {
                if(!this.data.GrapeVarieties.Any(g => g.Id == grapeId))
                {
                    return false;
                }
            }

            return true;
        }

        public bool WineExists(string name, int manufactureYear, string manufacturerId, int colorId, int tasteId, int wineAreaId, IEnumerable<int> grapeVarieties)
        {
            var exists = true;

           if (data.Products.Any(p => p.Name == name && p.ManufactureYear == manufactureYear && p.Manufacturer.Id == manufacturerId && p.ColorId == colorId && p.TasteId == tasteId && p.WineAreaId == wineAreaId))
            {
                var grapeVarietiesToCompare = data.Products
                    .Where(p => p.Name == name && p.ManufactureYear == manufactureYear && p.Manufacturer.Id == manufacturerId && p.ColorId == colorId && p.TasteId == tasteId && p.WineAreaId == wineAreaId)
                    .Select(p => p.GrapeVarieties)
                    .FirstOrDefault();

                if(grapeVarietiesToCompare.Count() == grapeVarieties.Count())
                {
                    foreach (var grape in grapeVarietiesToCompare)
                    {
                        if (!grapeVarieties.Contains(grape.GrapeVarietyId))
                        {
                            exists = false;
                        }
                    }
                }               
            }

            return exists;
        }

        public bool UserIsManufacturer(string id) => data.Manufacturers
            .Any(m => m.UserId == id);

        private IEnumerable<ProductDetailsServiceModel> GetProducts(IQueryable<Product> productQuery) => productQuery
            .Select(p => new ProductDetailsServiceModel
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                Price = p.Price,
                Color = p.Color.Name,
                Taste = p.Taste.Name,
                ManufactureYear = p.ManufactureYear,
                ManufacturerName = p.Manufacturer.Name,
                InStock = p.InStock,
                WineAreaName = p.WineArea.Name,
                ManufacturerId = p.ManufacturerId,                 
            })
            .ToList();
    }
}
