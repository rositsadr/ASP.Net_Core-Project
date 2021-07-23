using System.Collections.Generic;
using Web.Models.Enums;

namespace Web.Services.Products
{
    public interface IProductService
    {
        public ProductSearchPageServiceModel All(string manufacturer, string color, string searchTerm, ProductsSort sorting, int currantPage, int productsPerRage);

        public IEnumerable<string> GetAllColors();

        public IEnumerable<string> GetAllManufacturers();

        public ProductDetailsServiceModel Details(string id);

        public void Delete(string id);
    }
}
