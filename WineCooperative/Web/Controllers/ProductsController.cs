using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Models;
using Web.Models.Products;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly UserManager<User> _userManager;

        public ProductsController(WineCooperativeDbContext data, UserManager<User> userManager)
        {
            this.data = data;
            _userManager = userManager;
        }

        public IActionResult Add() => View(new ProductAddingModel
        {
            WineAreas = this.GetWineAreas(),
            AllGrapeVarieties = this.GetAllGrapeVarieties()           
        });

        [HttpPost]
        public IActionResult Add(ProductAddingModel wine)
        {
            if (!this.data.WineAreas.Any(w=>w.Id==wine.WineAreaId))
            {
                this.ModelState.AddModelError(nameof(wine.WineAreaId), "This wine area does not exists!");
            }

            foreach (var grapeId in wine.GrapeVarieties)
            {
                if(!this.data.GrapeVarieties.Any(g=>g.Id == grapeId))
                {
                    this.ModelState.AddModelError(nameof(wine.GrapeVarieties), "The grape variety you have chosen does not exists!");
                }
            }

            if(!ModelState.IsValid)
            {
                wine.WineAreas = this.GetWineAreas();
                wine.AllGrapeVarieties = this.GetAllGrapeVarieties();

                return View(wine);
            }

            var grapeViraieties = data.GrapeVarieties
                .Where(gv => wine.GrapeVarieties.Contains(gv.Id))
                .ToList();

            var user =_userManager.GetUserAsync(HttpContext.User).Result;

            var manifacturerId = data.UserAdditionalInformation
                .Where(a => a.Id == user.UserDataId)
                .Select(a => a.ManufacturerId)
                .FirstOrDefault();

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
                ManufacturerId = manifacturerId
            };

            data.Products.Add(product);
            data.SaveChanges();

            return RedirectToAction("All","Products");
        }

        public IActionResult All()
        {
            var products = data.Products
                 .Where(p => p.InStock)
                 .Select(p => new ProductViewModel
                 {
                     Id = p.Id,
                     ImageUrl = p.ImageUrl,
                     Name = p.Name,
                     Price = p.Price,
                 })
                 .ToList();

            return View(products);
        }

        public IActionResult ProductDisplay(string Id)
        {
            var product = data.Products
                .Where(p => p.Id == Id)
                .Select(p => new ProductDisplayModel
                {
                    Id=p.Id,
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
        private IEnumerable<ProductWineAreaModel> GetWineAreas() => this.data.WineAreas
            .Select(wa => new ProductWineAreaModel
            {
                WineAreaId = wa.Id,
                WineAreName = wa.Name
            });

        private IEnumerable<ProductGrapeVarieties> GetAllGrapeVarieties() => this.data.GrapeVarieties
            .Select(gv => new ProductGrapeVarieties
            {
                 GrapeVarietyId = gv.Id,
                 GrapeVarietyName = gv.Name
            });

    }
}
