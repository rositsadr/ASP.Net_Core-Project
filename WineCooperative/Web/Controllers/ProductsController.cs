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

            var manifacturer = data.UserAdditionalInformation
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
                ManufacturerId = manifacturer
            };

            data.Products.Add(product);
            data.SaveChanges();

            return RedirectToAction("All","Products");
        }

        public IActionResult All()
        {
            /* var products = data.Products
                 .Where(p => p.InStock)
                 .Select(p => new ProductViewModel
                 {

                 })
                 .ToList();*/

            return View();
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
