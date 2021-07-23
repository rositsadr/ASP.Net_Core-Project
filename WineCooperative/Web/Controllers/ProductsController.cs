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
            if(!(this.UserIsManufacturer() || this.User.IsInRole("Admin")))
            {
                return BadRequest();
            }

            return View(new ProductAddingModel
            {
                WineAreas = this.GetAllWineAreas(),
                AllGrapeVarieties = this.GetAllGrapeVarieties(),
                Manufacturers = this.GetManufacturers(),
                AllColors = this.GetAllColors(),
                AllTastes = this.GetAllTastes(),
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ProductAddingModel wine)
        {
            if(!this.data.ProductTastes.Any(pt=>pt.Id == wine.TasteId))
            {
                this.ModelState.AddModelError(nameof(wine.TasteId), "This is not existing wine taste.");
            }

            if (!this.data.ProductColors.Any(pc=>pc.Id == wine.ColorId))
            {
                this.ModelState.AddModelError(nameof(wine.ColorId), "This is not existing wine color.");
            }

            if (!this.data.WineAreas.Any(w=>w.Id == wine.WineAreaId))
            {
                this.ModelState.AddModelError(nameof(wine.WineAreaId), "This wine area does not exists!");
            }

            if(!this.data.Manufacturers.Any(m=>m.Id == wine.ManufacturerId))
            {
                this.ModelState.AddModelError(nameof(wine.ManufacturerId), "The Manufacturer does not exists.");
            }

            foreach (var grapeId in wine.GrapeVarieties)
            {
                if(!this.data.GrapeVarieties.Any(g=>g.Id == grapeId))
                {
                    this.ModelState.AddModelError(nameof(wine.GrapeVarieties), "The grape variety you have chosen does not exists!");
                }
            }

            if (data.Products.Any(p => p.Name == wine.Name && p.ManufactureYear == wine.ManufactureYear && p.Manufacturer.Id == wine.ManufacturerId && p.ColorId == wine.ColorId && p.TasteId == wine.TasteId))
            {
                this.ModelState.AddModelError(nameof(wine), "This wine is already in the list. Check your product list.");
            }

            if (!ModelState.IsValid)
            {
                wine.WineAreas = this.GetAllWineAreas();

                wine.AllGrapeVarieties = this.GetAllGrapeVarieties();

                wine.Manufacturers = this.GetManufacturers();

                wine.AllColors = this.GetAllColors();

                wine.AllTastes = this.GetAllTastes();

                return View(wine);
            }

            var product = new Product
            {
                Name = wine.Name,
                Price = wine.Price,
                ImageUrl = wine.ImageUrl,
                ManufactureYear = wine.ManufactureYear,
                Description = wine.Description,
                InStock = wine.InStock,
                WineAreaId = wine.WineAreaId,
                GrapeVarieties = new List<ProductGrapeVariety>(),
                ManufacturerId = wine.ManufacturerId,
                TasteId = wine.TasteId,
                ColorId = wine.ColorId
            };

            foreach (var grapeId in wine.GrapeVarieties)
            {
                product.GrapeVarieties.Add(new ProductGrapeVariety
                {
                    ProductId = product.Id,
                    GrapeVarietyId = grapeId
                });
            }
            data.Products.Add(product);
            data.SaveChanges();

            return RedirectToAction("All","Products");
        }

        public IActionResult All([FromQuery] ProductSearchPageViewModel query, string id = null)
        {
            var productsResult = this.productService.All(query.Manufacturer, query.Color, query.SearchTerm, query.Sorting, query.CurrantPage, ProductSearchPageViewModel.productsPerPage);

            if(id!=null)
            {
                productsResult.Products = productsResult.Products
                    .Where(p => p.ManufacturerId == id)
                    .ToList();
            }
            else
            {
                productsResult.Products = productsResult.Products
                    .Where(p => p.InStock)
                    .ToList();
            }

            var manufacturers = this.productService.GetAllManufacturers();

            var colors = this.productService.GetAllColors();

            query.Colors = colors;

            query.Manufacturers = manufacturers;

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
            var product = data.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductAddingModel
                {
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    ManufactureYear = p.ManufactureYear,
                    InStock = p.InStock,
                    ColorId = p.ColorId,
                    TasteId = p.TasteId,
                    ManufacturerId = p.ManufacturerId,
                    WineAreaId = p.WineAreaId,
                    GrapeVarieties = p.GrapeVarieties.Select(gv=>gv.GrapeVarietyId)
                })
                .FirstOrDefault();

            product.Manufacturers = GetManufacturers();

            product.AllColors = GetAllColors();

            product.AllTastes = GetAllTastes();

            product.AllGrapeVarieties = GetAllGrapeVarieties();

            product.WineAreas = GetAllWineAreas();

            ViewBag.Id = id;

            return View("Add",product);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(ProductAddingModel wine, string id)
        {
            var product = data.Products.Where(p => p.Id == id).FirstOrDefault();

            if (product.Name != wine.Name)
            {
                product.Name = wine.Name;
            }

            if (product.Price != wine.Price)
            {
                product.Price = wine.Price;
            }

            if(product.TasteId != wine.TasteId)
            {
                product.TasteId = wine.TasteId;
            }

            if(product.ColorId != wine.ColorId)
            {
                product.ColorId = wine.ColorId;
            }

            if (product.ImageUrl != wine.ImageUrl)
            {
                product.ImageUrl = wine.ImageUrl;
            }

            if (product.Description != wine.Description)
            {
                product.Description = wine.Description;
            }

            if (product.InStock != wine.InStock)
            {
                product.InStock = wine.InStock;
            }

            if (product.ManufacturerId != wine.ManufacturerId)
            {
                product.ManufacturerId = wine.ManufacturerId;
            }

            if (product.ManufactureYear != wine.ManufactureYear)
            {
                product.ManufactureYear = wine.ManufactureYear;
            }

            if (product.WineAreaId != wine.WineAreaId)
            {
                product.WineAreaId = wine.WineAreaId;
            }

            if(product.GrapeVarieties.Any(g=>!wine.GrapeVarieties.Contains(g.GrapeVarietyId)))
            {
                var toRemove = data.ProductGrapeVarieties
                    .Where(p => p.ProductId == id)
                    .ToList();

                data.ProductGrapeVarieties.RemoveRange(toRemove);

                product.GrapeVarieties = new List<ProductGrapeVariety>();

                foreach (var grapeId in wine.GrapeVarieties)
                {
                    product.GrapeVarieties.Add(new ProductGrapeVariety
                    {
                        ProductId = id,
                        GrapeVarietyId = grapeId,
                    });
                }
            }

            data.SaveChanges();

            return RedirectToAction("MyProducts", "Users");
        }

        private IEnumerable<ProductWineAreaModel> GetAllWineAreas() => this.data.WineAreas
            .Select(wa => new ProductWineAreaModel
            {
                WineAreaId = wa.Id,
                WineAreName = wa.Name
            });

        private IEnumerable<ProductGrapeVarietiesModel> GetAllGrapeVarieties() => this.data.GrapeVarieties
            .Select(gv => new ProductGrapeVarietiesModel
            {
                 GrapeVarietyId = gv.Id,
                 GrapeVarietyName = gv.Name
            });

        private IEnumerable<ProductManufacturerModel> GetManufacturers()
        {
            var manufacturers = this.data.Manufacturers
                .Select(m => new ProductManufacturerModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    UserId = m.UserId
                })
                .AsQueryable();

            if (User.IsInRole("Admin"))
            {
                return manufacturers;
            }

            return manufacturers.Where(m => m.UserId == User.GetId());
        }

        private IEnumerable<ProductColorViewModel> GetAllColors() => this.data.ProductColors
            .Select(m => new ProductColorViewModel
            {
                Id = m.Id,
                Name = m.Name
            });

        private IEnumerable<ProductTasteViewModel> GetAllTastes() => this.data.ProductTastes
            .Select(m => new ProductTasteViewModel
            {
                Id = m.Id,
                Name = m.Name
            });

        private bool UserIsManufacturer() => data.Manufacturers
            .Any(m => m.UserId == this.User.GetId());
    }
}
