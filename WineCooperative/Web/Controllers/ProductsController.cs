using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Infrastructures;
using Web.Models.Products;
using Web.Services.Manufacturers;
using Web.Services.Products;
using static Web.WebConstants;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService productService;
        private readonly IManufacturerService manufacturerService;
        private readonly IMapper mapper;

        public ProductsController(IProductService productService, IManufacturerService manufacturerService, IMapper mapper)
        {
            this.productService = productService;
            this.manufacturerService = manufacturerService;
            this.mapper = mapper;
        }

        [Authorize]
        public IActionResult Add()
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            if (User.IsMember())
            {
                return View(new ProductModel
                {
                    WineAreas = this.productService.GetAllWineAreas(),
                    AllGrapeVarieties = this.productService.GetAllGrapeVarieties(),
                    Manufacturers = this.manufacturerService.ManufacturersNameByUser(User.GetId()),
                    AllColors = this.productService.GetAllColors(),
                    AllTastes = this.productService.GetAllTastes(),
                });
            }

            return RedirectToAction("BecomeMember", "Users");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ProductModel product)
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }
           
            if(User.IsMember())
            {
                if (!productService.TasteExists(product.TasteId))
                {
                    this.ModelState.AddModelError(nameof(product.TasteId), "This is not existing wine taste.");
                }

                if (!productService.ColorExists(product.ColorId))
                {
                    this.ModelState.AddModelError(nameof(product.ColorId), "This is not existing wine color.");
                }

                if (!productService.WineAreaExists(product.WineAreaId))
                {
                    this.ModelState.AddModelError(nameof(product.WineAreaId), "This wine area does not exists!");
                }

                if (!manufacturerService.ManufacturerExistsById(product.ManufacturerId))
                {
                    this.ModelState.AddModelError(string.Empty, "The Manufacturer does not exists.");
                }

                if (!manufacturerService.ManufacturersNameByUser(User.GetId()).Any(m => m.Id == product.ManufacturerId))
                {
                    this.ModelState.AddModelError(string.Empty, "The Manufacturer you have choosen is not allowed. Choose one of yours.");
                }

                if (!productService.GrapeVarietiesExists(product.GrapeVarieties))
                {
                    this.ModelState.AddModelError(nameof(product.GrapeVarieties), "The grape variety you have chosen does not exists!");
                }

                if (productService.WineExists(product.Name, product.ManufactureYear, product.ManufacturerId, product.ColorId, product.TasteId, product.WineAreaId, product.GrapeVarieties))
                {
                    this.ModelState.AddModelError(string.Empty, "This wine is already in the list. Check your product list.");
                }

                if (!ModelState.IsValid)
                {
                    product.WineAreas = this.productService.GetAllWineAreas();
                    product.AllGrapeVarieties = this.productService.GetAllGrapeVarieties();
                    product.Manufacturers = this.manufacturerService.ManufacturersNameByUser(User.GetId());
                    product.AllColors = this.productService.GetAllColors();
                    product.AllTastes = this.productService.GetAllTastes();

                    return View(product);
                }

                this.productService.CreateProduct(product.Name, product.Price, product.ImageUrl, product.ManufactureYear, product.Description, product.InStock, product.WineAreaId, product.ManufacturerId, product.TasteId, product.ColorId, product.GrapeVarieties);

                return RedirectToAction("All", "Products");
            }

            return RedirectToAction("BecomeMember", "Users");
        }

        public IActionResult All([FromQuery] ProductSearchPageModel query, string id = null)
        {
            var productsResult = this.productService.All(query.Color, query.SearchTerm, query.Sorting, query.CurrantPage, ProductSearchPageModel.productsPerPage);

            if(!User.IsAdmin())
            {
                productsResult.Products = productsResult.Products
                    .Where(p => p.InStock);
            }

            if(id!=null)
            {
                productsResult.Products = productsResult.Products
                    .Where(p => p.ManufacturerId == id)
                    .ToList();
            }

            var colors = this.productService.GetAllColorsName();
            query.Colors = colors;
            query.TotalProducts = productsResult.TotalProducts;
            query.Products = productsResult.Products;

            return View(query);
        }

        [Authorize]
        public IActionResult Edit(string id)
        {
            var userId = User.GetId();

            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            var product = this.productService.Edit(id);

            if (product.UserId != userId && !this.User.IsInRole(AdministratorRole))
            {
                return Unauthorized();
            }

            var productToEdit = mapper
                .Map<ProductModel>(product);

            var manufacturers = this.manufacturerService.AllManufacturers();

            if (User.IsMember())
            {
                manufacturers = this.manufacturerService.ManufacturersNameByUser(userId);
            }

            productToEdit.AllColors = this.productService.GetAllColors();
            productToEdit.AllTastes = this.productService.GetAllTastes();
            productToEdit.AllGrapeVarieties = this.productService.GetAllGrapeVarieties();
            productToEdit.WineAreas = this.productService.GetAllWineAreas();
            productToEdit.Manufacturers = manufacturers;

            return View(productToEdit);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(ProductModel product, string id)
        {
            var userId = User.GetId();

            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            if (!productService.TasteExists(product.TasteId))
            {
                this.ModelState.AddModelError(nameof(product.TasteId), "This is not existing wine taste.");
            }

            if (!productService.ColorExists(product.ColorId))
            {
                this.ModelState.AddModelError(nameof(product.ColorId), "This is not existing wine color.");
            }

            if (!productService.WineAreaExists(product.WineAreaId))
            {
                this.ModelState.AddModelError(nameof(product.WineAreaId), "This wine area does not exists!");
            }

            if (!manufacturerService.ManufacturerExistsById(product.ManufacturerId))
            {
                this.ModelState.AddModelError(nameof(product.ManufacturerId), "The Manufacturer does not exists.");
            }

            if (!productService.GrapeVarietiesExists(product.GrapeVarieties))
            {
                this.ModelState.AddModelError(nameof(product.GrapeVarieties), "The grape variety you have chosen does not exists!");
            }

            if (!productService.WineExists(product.Name, product.ManufactureYear, product.ManufacturerId, product.ColorId, product.TasteId, product.WineAreaId, product.GrapeVarieties))
            {
                this.ModelState.AddModelError(string.Empty, "This wine is not in the list. Add it first.");
            }

            if (!ModelState.IsValid)
            {
                product.WineAreas = this.productService.GetAllWineAreas();
                product.AllGrapeVarieties = this.productService.GetAllGrapeVarieties();
                product.Manufacturers = this.manufacturerService.ManufacturersNameByUser(userId);
                product.AllColors = this.productService.GetAllColors();
                product.AllTastes = this.productService.GetAllTastes();

                return View(product);
            }

            if (!(this.productService.IsItUsersProduct(userId, id) || User.IsAdmin()))
            {
                return BadRequest();
            }

            this.productService.ApplyChanges(id, product.Name, product.Price, product.ImageUrl, product.ManufactureYear, product.Description, product.InStock, product.WineAreaId, product.ManufacturerId, product.TasteId, product.ColorId, product.GrapeVarieties);

            return RedirectToAction("MyProducts", "Users");
        }

        public IActionResult Details(string id)
        {
            var product = productService.Details(id);

            if(product==null)
            {
                return RedirectToAction("All");
            }

            return View(product);
        }

        [Authorize]
        public IActionResult Delete(string id)
        {
            var userId = User.GetId();

            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            if (!(this.productService.IsItUsersProduct(userId, id) || User.IsAdmin()))
            {
                return Unauthorized();
            }

            this.productService.Delete(id);

            return RedirectToAction("All");
        }      
    }
}
