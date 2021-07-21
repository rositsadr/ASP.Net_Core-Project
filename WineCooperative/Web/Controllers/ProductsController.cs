using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Enums;
using Web.Models.Products;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WineCooperativeDbContext data;

        public ProductsController(WineCooperativeDbContext data)
        {
            this.data = data;
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
                WineAreas = this.GetWineAreas(),
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
                wine.WineAreas = this.GetWineAreas();

                wine.AllGrapeVarieties = this.GetAllGrapeVarieties();

                wine.Manufacturers = this.GetManufacturers();

                wine.AllColors = this.GetAllColors();

                wine.AllTastes = this.GetAllTastes();

                return View(wine);
            }

            var grapeViraieties = data.GrapeVarieties
                    .Where(gv => wine.GrapeVarieties.Contains(gv.Id))
                    .ToList();

            var product = new Product
            {
                Name = wine.Name,
                Price = wine.Price,
                ImageUrl = wine.ImageUrl,
                ManufactureYear = wine.ManufactureYear,
                Description = wine.Description,
                InStock = wine.InStock,
                WineAreaId = wine.WineAreaId,
                GrapeVarieties = grapeViraieties,
                ManufacturerId = wine.ManufacturerId,
                TasteId = wine.TasteId,
                ColorId = wine.ColorId
            };

            data.Products.Add(product);
            data.SaveChanges();

            return RedirectToAction("All","Products");
        }

        public IActionResult All([FromQuery] ProductSearchPageViewModel query)
        {
            var productsQuery = data.Products.AsQueryable();

            if (!string.IsNullOrEmpty(query.Manufacturer))
            {
                productsQuery = productsQuery
                    .Where(p => p.Manufacturer.Name == query.Manufacturer);
            }
            
            if (!string.IsNullOrEmpty(query.Color))
            {
                productsQuery = productsQuery
                    .Where(p => p.Color.Name == query.Color);
            }

            if(!string.IsNullOrEmpty(query.SearchTerm))
            {
                productsQuery = productsQuery
                    .Where(p =>(p.Name.ToLower() + " " + p.Description.ToLower()).Contains(query.SearchTerm));         
            }

            productsQuery = query.Sorting switch
            {
                ProductsSort.ByYear => productsQuery.OrderByDescending(p=>p.ManufactureYear),
                ProductsSort.ByManufacturer => productsQuery.OrderBy(p=>p.Manufacturer.Name),
                ProductsSort.ByName or _ => productsQuery.OrderBy(p=>p.Name)
            };

            var totalProducts = productsQuery.Count();

            var products = productsQuery
                 .Where(p => p.InStock)
                 .Skip((query.CurrantPage-1) * ProductSearchPageViewModel.productsPerPage)
                 .Take(ProductSearchPageViewModel.productsPerPage)
                 .Select(p => new ProductViewModel
                 {
                     Id = p.Id,
                     ImageUrl = p.ImageUrl,
                     Name = p.Name,
                     Price = p.Price,
                 })
                 .ToList();

            query.TotalProducts = totalProducts;

            query.Colors = data.ProductColors
                .Select(c=>c.Name);

            query.Manufacturers = data.Manufacturers
                .Select(m => m.Name);

            query.Products = products;

            return View(query);
        }

        public IActionResult Details(string id)
        {
            if(!ExistingProductCheck(id))
            {
                var product = data.Products
               .Where(p => p.Id == id)
               .Select(p => new ProductDisplayModel
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

                return View(product);
            }

            return RedirectToAction("All");
        }

        public IActionResult Delete(string id)
        {
            if(ExistingProductCheck(id))
            {
                var wine = data.Products
                    .Find(id);

                data.Remove(wine);
                data.SaveChanges();
            }

           return RedirectToAction("All");
        }

        public IActionResult Edit(string id)
        {
            return View();
        }

        private IEnumerable<ProductWineAreaModel> GetWineAreas() => this.data.WineAreas
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

        private IEnumerable<ProductColorViewModel> GetAllColors() =>
            this.data.ProductColors
            .Select(m => new ProductColorViewModel
            {
                Id = m.Id,
                Name = m.Name
            });

        private bool UserIsManufacturer()=>
            data.Manufacturers
                .Any(m => m.UserId == this.User.GetId());

        private IEnumerable<ProductTasteViewModel> GetAllTastes() =>
            this.data.ProductTastes
            .Select(m => new ProductTasteViewModel
            {
                Id = m.Id,
                Name = m.Name
            });

        private bool ExistingProductCheck(string wineId)
        {
            bool exists = false;

            if (data.Products.Any(p => p.Id == wineId))
            {
                //Todo:
                exists = true;
            }
            return exists;
        }
    }
}
