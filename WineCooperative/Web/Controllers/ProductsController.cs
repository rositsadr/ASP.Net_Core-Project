using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Infrastructures;
using Web.ViewModels.Products;
using Web.Services.Manufacturers;
using Web.Services.Products;
using Web.Services.Products.Models;
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

            this.TempData[ErrorMessageKey] = NotPermitted;
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

                if (productService.WineExists(product.Name, product.ManufactureYear, product.ManufacturerId, product.ColorId, product.TasteId, product.WineAreaId, product.GrapeVarieties, product.ImageUrl))
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

                this.TempData[SuccessMessageKey] = string.Format(SuccesssfulyAdded, "product");
                return RedirectToAction("All", "Products");
            }

            this.TempData[ErrorMessageKey] = NotPermitted;
            return RedirectToAction("BecomeMember", "Users");
        }

        public IActionResult All([FromQuery] ProductSearchPageModel query, string id = null)
        {
            var productsResult = new ProductSearchPageServiceModel();

            if (User.IsAdmin())
            {
                productsResult = this.productService.All(query.Color, query.SearchTerm, query.Sorting, query.CurrantPage, ProductSearchPageModel.productsPerPage);
            }
            else
            {
                productsResult = this.productService.AllInStock(query.Color, query.SearchTerm, query.Sorting, query.CurrantPage, ProductSearchPageModel.productsPerPage);
            }

            if(productsResult == null)
            {
                return BadRequest();
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
                this.TempData[ErrorMessageKey] = NotPermitted;
                return RedirectToAction("BecomeMember", "Users");
            }

            var product = this.productService.Edit(id);

            if(product == null)
            {
                return BadRequest();
            }

            if (product.UserId != userId && !this.User.IsAdmin())
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
                this.TempData[ErrorMessageKey] = NotPermitted;
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

            if (productService.WineExists(product.Name, product.ManufactureYear, product.ManufacturerId, product.ColorId, product.TasteId, product.WineAreaId, product.GrapeVarieties, product.ImageUrl) && product.InStock)
            {
                this.ModelState.AddModelError(string.Empty, "This wine is already in the list.");
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

            if (!(this.productService.IsUsersProduct(userId, id) || User.IsAdmin()))
            {
                return BadRequest();
            }

            this.productService.ApplyChanges(id, product.Name, product.Price, product.ImageUrl, product.ManufactureYear, product.Description, product.InStock, product.WineAreaId, product.ManufacturerId, product.TasteId, product.ColorId, product.GrapeVarieties);

            this.TempData[SuccessMessageKey] = string.Format(SuccesssfulyEdited,"product");
            return RedirectToAction("All");
        }

        public IActionResult Details(string id)
        {
            var product = productService.Details(id);

            if(product==null)
            {
                this.TempData[ErrorMessageKey] = "The product you are trying to view is not in the list!";
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
                this.TempData[ErrorMessageKey] = NotPermitted;
                return RedirectToAction("BecomeMember", "Users");
            }

            if (!this.productService.IsUsersProduct(userId, id))
            {
                return Unauthorized();
            }

            var success = this.productService.Delete(id);

            if (success)
            {
                this.TempData[SuccessMessageKey] = string.Format(SuccessfullyDeleted, "product");
            }
            else
            {
                this.TempData[ErrorMessageKey] = string.Format(NotExistToDelete,"Product");
            }

            return RedirectToAction("All");
        }      
    }
}
