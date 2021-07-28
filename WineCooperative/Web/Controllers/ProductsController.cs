using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Products;
using Web.Services.Products;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WineCooperativeDbContext data;

        private readonly IProductService productService;

        public ProductsController(WineCooperativeDbContext data, IProductService productService)
        {
            this.data = data;
            this.productService = productService;
        }

        [Authorize]
        public IActionResult Add()
        {
            if(!(this.productService.UserIsManufacturer(User.GetId()) || this.User.IsInRole("Admin")))
            {
                return BadRequest();
            }

            return View(new ProductModel
            {
                WineAreas = this.productService.GetAllWineAreas(),
                AllGrapeVarieties = this.productService.GetAllGrapeVarieties(),
                Manufacturers = this.productService.GetAllManufacturers(),
                AllColors = this.productService.GetAllColors(),
                AllTastes = this.productService.GetAllTastes(),
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ProductModel product)
        {
            if(!productService.TasteExists(product.TasteId))
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

            if(!productService.ManufacturerExists(product.ManufacturerId))
            {
                this.ModelState.AddModelError(nameof(product.ManufacturerId), "The Manufacturer does not exists.");
            }

            if (!productService.GrapeVarietiesExists(product.GrapeVarieties))
            {
                this.ModelState.AddModelError(nameof(product.GrapeVarieties), "The grape variety you have chosen does not exists!");
            }

            if (productService.WineExists(product.Name, product.ManufactureYear,product.ManufacturerId, product.ColorId, product.TasteId, product.WineAreaId, product.GrapeVarieties))
            {
                this.ModelState.AddModelError(string.Empty, "This wine is already in the list. Check your product list.");
            }

            if (!ModelState.IsValid)
            {
                product.WineAreas = this.productService.GetAllWineAreas();

                product.AllGrapeVarieties = this.productService.GetAllGrapeVarieties();

                product.Manufacturers = this.productService.GetAllManufacturers();

                product.AllColors = this.productService.GetAllColors();

                product.AllTastes = this.productService.GetAllTastes();

                return View(product);
            }

            this.productService.CreateProduct(product.Name, product.Price, product.ImageUrl, product.ManufactureYear, product.Description, product.InStock, product.WineAreaId, product.ManufacturerId, product.TasteId, product.ColorId, product.GrapeVarieties);

            return RedirectToAction("All","Products");
        }

        public IActionResult All([FromQuery] ProductSearchPageViewModel query, string id = null)
        {
            var productsResult = this.productService.All(query.Color, query.SearchTerm, query.Sorting, query.CurrantPage, ProductSearchPageViewModel.productsPerPage);

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

        public IActionResult Details(string id)
        {
            var product = productService.Details(id);

            if(product==null)
            {
                return RedirectToAction("All");
            }

            return View(product);
        }

        public IActionResult Delete(string id)
        {
            this.productService.Delete(id);

            return RedirectToAction("All");
        }

        [Authorize]
        public IActionResult Edit(string id)
        {
            var userId = User.GetId();

            if (!(this.productService.UserIsManufacturer(userId) || this.User.IsInRole("Admin")))
            {
                return BadRequest();
            }

            var product = this.productService.Edit(id);

            if (product.UserId != userId)
            {
                return Unauthorized();
            }

            var productToEdit = new ProductModel
                {
                    Name = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Description = product.Description,
                    ManufactureYear = product.ManufactureYear,
                    InStock = product.InStock,
                    ColorId = product.ColorId,
                    TasteId = product.TasteId,
                    ManufacturerId = product.ManufacturerId,
                    WineAreaId = product.WineAreaId,
                    GrapeVarieties = product.GrapeVarieties
                };

            var manufacturers = this.productService.GetAllManufacturers();

            if (User.IsInRole("Member"))
            {
                manufacturers = manufacturers
                    .Where(m => m.UserId == userId);
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
            var productToEdit = data.Products.Where(p => p.Id == id).FirstOrDefault();

            if (productToEdit.Name != product.Name)
            {
                productToEdit.Name = product.Name;
            }

            if (productToEdit.Price != product.Price)
            {
                productToEdit.Price = product.Price;
            }

            if(productToEdit.TasteId != product.TasteId)
            {
                productToEdit.TasteId = product.TasteId;
            }

            if(productToEdit.ColorId != product.ColorId)
            {
                productToEdit.ColorId = product.ColorId;
            }

            if (productToEdit.ImageUrl != product.ImageUrl)
            {
                productToEdit.ImageUrl = product.ImageUrl;
            }

            if (productToEdit.Description != product.Description)
            {
                productToEdit.Description = product.Description;
            }

            if (productToEdit.InStock != product.InStock)
            {
                productToEdit.InStock = product.InStock;
            }

            if (productToEdit.ManufacturerId != product.ManufacturerId)
            {
                productToEdit.ManufacturerId = product.ManufacturerId;
            }

            if (productToEdit.ManufactureYear != product.ManufactureYear)
            {
                productToEdit.ManufactureYear = product.ManufactureYear;
            }

            if (productToEdit.WineAreaId != product.WineAreaId)
            {
                productToEdit.WineAreaId = product.WineAreaId;
            }

            if(productToEdit.GrapeVarieties.Any(g=>!product.GrapeVarieties.Contains(g.GrapeVarietyId)))
            {
                var toRemove = data.ProductGrapeVarieties
                    .Where(p => p.ProductId == id)
                    .ToList();

                data.ProductGrapeVarieties.RemoveRange(toRemove);

                productToEdit.GrapeVarieties = new List<ProductGrapeVariety>();

                foreach (var grapeId in product.GrapeVarieties)
                {
                    productToEdit.GrapeVarieties.Add(new ProductGrapeVariety
                    {
                        ProductId = id,
                        GrapeVarietyId = grapeId,
                    });
                }
            }

            data.SaveChanges();

            return RedirectToAction("MyProducts", "Users");
        }      
    }
}
