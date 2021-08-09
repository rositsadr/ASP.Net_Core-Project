using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Models;
using Web.Models.Products.Enums;
using Web.Services.Products.Models;

namespace Web.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly WineCooperativeDbContext data;
        private readonly IConfigurationProvider config;

        public ProductService(WineCooperativeDbContext data, IMapper mapper)
        {
            this.data = data;
            this.config = mapper.ConfigurationProvider;
        }

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
                 .ProjectTo<ProductServiceModel>(config)
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

        public ProductEditServiceModel Edit(string productId) => data.Products
                 .Where(p => p.Id == productId)
                 .ProjectTo<ProductEditServiceModel>(config)
                 .FirstOrDefault();

        public bool ApplyChanges(string productId, string name, decimal price, string imageUrl, int manufactureYear, string description, bool inStock, int wineAreaId, string manufacturerId, int tasteId, int colorId, IEnumerable<int> grapeVarieties)
        {
            var product = data.Products
                .Where(p => p.Id == productId)
                .FirstOrDefault();

            if(product == null)
            {
                return false;
            }

            product.Name = name;
            product.Price = price;
            product.ImageUrl = imageUrl;
            product.ManufactureYear = manufactureYear;
            product.Description = description;
            product.InStock = inStock;
            product.WineAreaId = wineAreaId;
            product.ManufacturerId = manufacturerId;
            product.TasteId = tasteId;
            product.ColorId = colorId;

            if (product.GrapeVarieties.Any(g => !grapeVarieties.Contains(g.GrapeVarietyId)))
            {
                var toRemove = data.ProductGrapeVarieties
                    .Where(p => p.ProductId == productId)
                    .ToList();

                data.ProductGrapeVarieties.RemoveRange(toRemove);

                product.GrapeVarieties = new List<ProductGrapeVariety>();

                foreach (var grapeId in grapeVarieties)
                {
                    product.GrapeVarieties.Add(new ProductGrapeVariety
                    {
                        ProductId = productId,
                        GrapeVarietyId = grapeId,
                    });
                }
            }

            data.SaveChanges();

            return true;
        }

        public ProductDetailsServiceModel Details(string productId) => this.GetProducts(data.Products
            .Where(p => p.Id == productId))
            .FirstOrDefault();

        public bool Delete(string productId)
        {
            var product = data.Products
                .Find(productId);

            if (product != null)
            {
                var productGrapeVarieties = data.ProductGrapeVarieties.Where(p => p.ProductId == productId).ToList();

                data.ProductGrapeVarieties.RemoveRange(productGrapeVarieties);
                data.Products.Remove(product);
                data.SaveChanges();

                return true;
            }

            return false;
        }

        public IEnumerable<string> GetAllColorsName() => data.ProductColors
                .Select(c => c.Name);

        public IEnumerable<ProductWineAreaServiceModel> GetAllWineAreas() => this.data.WineAreas
          .ProjectTo<ProductWineAreaServiceModel>(config);

        public IEnumerable<ProductGrapeVarietiesServiceModel> GetAllGrapeVarieties() => this.data.GrapeVarieties
            .ProjectTo<ProductGrapeVarietiesServiceModel>(config);

        public IEnumerable<ProductColorServiceModel> GetAllColors() => this.data.ProductColors
            .ProjectTo<ProductColorServiceModel>(config);

        public IEnumerable<ProductTasteServiceModel> GetAllTastes() => this.data.ProductTastes
            .ProjectTo<ProductTasteServiceModel>(config);

        public bool ColorExists(int colorId) => this.data.ProductColors
            .Any(pc => pc.Id == colorId);

        public bool TasteExists(int tasteId) => this.data.ProductTastes
            .Any(pt => pt.Id == tasteId);

        public bool WineAreaExists(int wineAreaId) => this.data.WineAreas
            .Any(w => w.Id == wineAreaId);

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
            if (data.Products.Any(p => p.Name == name && p.ManufactureYear == manufactureYear && p.Manufacturer.Id == manufacturerId && p.ColorId == colorId && p.TasteId == tasteId && p.WineAreaId == wineAreaId && p.GrapeVarieties.Count() == grapeVarieties.Count()))
            {
                var grapeVarietiesToCompare = data.Products
                    .Where(p => p.Name == name && p.ManufactureYear == manufactureYear && p.Manufacturer.Id == manufacturerId && p.ColorId == colorId && p.TasteId == tasteId && p.WineAreaId == wineAreaId)
                    .Select(p => p.GrapeVarieties)
                    .FirstOrDefault();

                var countSameGrape = 0;

                foreach (var grape in grapeVarietiesToCompare)
                {
                    if (grapeVarieties.Contains(grape.GrapeVarietyId))
                    {
                        countSameGrape++;
                    }
                }

                if (countSameGrape == grapeVarieties.Count())
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsUsersProduct(string userId, string productId) => data.Products
            .Any(p => p.Id == productId && p.Manufacturer.UserId == userId);

        private IEnumerable<ProductDetailsServiceModel> GetProducts(IQueryable<Product> productQuery) => productQuery
            .ProjectTo<ProductDetailsServiceModel>(config)
            .ToList();
    }
}
