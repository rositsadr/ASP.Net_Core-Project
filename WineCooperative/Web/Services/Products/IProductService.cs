using System.Collections.Generic;
using Web.Models.Products.Enums;
using Web.Services.Products.Models;

namespace Web.Services.Products
{
    public interface IProductService
    {
        public string CreateProduct(string name, decimal price, string imageUrl, int manufactureYear, string description, bool inStock, int wineAreaId, string manufacturerId, int tasteId, int colorId, IEnumerable<int> grapeVarieties);

        public ProductSearchPageServiceModel All(string color, string searchTerm, ProductsSort sorting, int currantPage, int productsPerRage);

        public IEnumerable<ProductDetailsServiceModel> ProductsByUser(string userId);

        public ProductEditServiceModel Edit(string productid);

        public bool ApplyChanges(string productId, string name, decimal price, string imageUrl, int manufactureYear, string description, bool inStock, int wineAreaId, string manufacturerId, int tasteId, int colorId, IEnumerable<int> grapeVarieties);

        public ProductDetailsServiceModel Details(string productId);

        public bool Delete(string productId);

        public IEnumerable<string> GetAllColorsName();

        public IEnumerable<ProductWineAreaServiceModel> GetAllWineAreas();

        public IEnumerable<ProductGrapeVarietiesServiceModel> GetAllGrapeVarieties();

        public IEnumerable<ProductColorServiceModel> GetAllColors();

        public IEnumerable<ProductTasteServiceModel> GetAllTastes();

        public bool ColorExists(int colorId);

        public bool TasteExists(int tasteId);

        public bool WineAreaExists(int wineAreaId);

        public bool GrapeVarietiesExists(IEnumerable<int> grapeVarieties);

        public bool WineExists(string name, int manufactureYear, string manufacturerId, int colorId, int tasteId, int wineAreaId, IEnumerable<int> grapeVarieties);

        public bool IsUsersProduct(string userId, string productId);
    }
}
