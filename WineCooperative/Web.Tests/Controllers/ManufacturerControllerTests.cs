using MyTested.AspNetCore.Mvc;
using System.Linq;
using Web.Controllers;
using Web.Models;
using Web.Models.Manufacturers;
using Web.Services.Manufacturers.Models;
using Xunit;
using static Web.WebConstants;
using static Web.Tests.Data.Manufacturers;
using System.Collections.Generic;
using Web.Data.Models;

namespace Web.Tests.Controllers
{
    public class ManufacturerControllerTests
    {
        [Fact]
        public void GetAddActionShouldReturnViewIfUserIsMember() =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath("/Manufacturers/Add")
                .WithMethod(HttpMethod.Get)
                .WithUser(user => user.InRole(MemberRole)))
            .To<ManufacturersController>(c => c.Add())
            .Which()
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View();

        [Fact]
        public void PostAddActionRoute() => 
            MyRouting
            .Configuration()
            .ShouldMap(request=> request
                .WithPath("/Manufacturers/Add")
                .WithMethod(HttpMethod.Post))
            .To<ManufacturersController>(c=>c.Add(With
                    .Any<ManufacturerModel>()));

        [Theory]
        [InlineData("Winary", "00359876543", "winery@abv.bg", true, "Kovachev", "3124", "Spasovo")]
        public void PostAddActionShouldAddManufacturerToDataIfModelStateIsValid(string name, string phoneNumber, string email, bool isFunctional, string street, string zipCode, string townName) =>
            MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(roles: MemberRole))
            .Calling(c => c.Add(new ManufacturerModel
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                IsFunctional = isFunctional,
                Address = new ManufacturerAddressViewModel
                {
                    Street = street,
                    ZipCode = zipCode,
                    TownName = townName,
                }
            }))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .Data(data => data
                .WithSet<Manufacturer>(manufacturers => manufacturers
                    .Any(m => m.Name == name && m.UserId == TestUser.Identifier)))
            .TempData(tempData=>tempData
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

        [Fact]
        public void AllActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Manufacturers/All"))
            .To<ManufacturersController>(c => c.All());

        [Fact]
        public void AllActionShouldReturnView() =>
            MyController<ManufacturersController>
            .Instance(controller => controller
                .WithData(FiveManufacturers))
            .Calling(c => c.All())
            .ShouldHave()
            .MemoryCache(cache => cache
                    .ContainingEntry(entry => entry
                        .WithKey(manufacturersCacheKey)
                        .WithValueOfType<List<ManufacturerServiceModel>>()))
                .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<ManufacturerServiceModel>>());
        //.Passing(m=>m.Count() == 5));

        [Fact]
        public void GetEditRouteWithRouteValue() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Manufacturers/Edit/{ManufacturerId}"))
            .To<ManufacturersController>(c => c.Edit(ManufacturerId));

        [Fact]
        public void GetEditshouldReturnViewWithData() =>
             MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(user=> user.InRole(MemberRole))
                .WithData(new Manufacturer() {UserId = TestUser.Identifier, Id = ManufacturerId}))
            .Calling(c => c.Edit(ManufacturerId))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ManufacturerModel>());

        [Fact]
        public void PostEditRouteWithRuteValue()=>
              MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Manufacturers/Edit/{ManufacturerId}")
                .WithMethod(HttpMethod.Post))
            .To<ManufacturersController>(c => c.Edit(With
                    .Any<ManufacturerModel>(), ManufacturerId));

        [Theory]
        [InlineData("Winary", "00359876543", "winery@abv.bg", true, "Kovachev", "3124", "Spasovo")]
        public void PostEditShouldReturnViewWithData(string name, string phoneNumber, string email, bool isFunctional, string street, string zipCode, string townName) =>
            MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(user => user.InRole(MemberRole))
                .WithData(new Manufacturer() { UserId = TestUser.Identifier, Id = ManufacturerId }))
            .Calling(c => c.Edit(new ManufacturerModel()
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                IsFunctional = isFunctional,
                Address = new ManufacturerAddressViewModel()
                {
                    Street = street,
                    ZipCode = zipCode,
                    TownName = townName
                }
            }, ManufacturerId))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .Data(data=>data
                .WithSet<Manufacturer>(m=>m.Any(m=>m.Id == ManufacturerId && m.Name == name && m.UserId == TestUser.Identifier)))
            .TempData(temp=>temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

        [Fact]
        public void DeleteRouteWithRouteValues() =>
            MyPipeline
            .Configuration()
            .ShouldMap(request=>request
                .WithPath($"/Manufacturers/Delete/{ManufacturerId}")
                .WithUser(TestUser.Identifier,TestUser.Username, MemberRole,AdministratorRole))
            .To<ManufacturersController>(c => c.Delete(ManufacturerId));

        [Fact]
        public void DeleteShouldChangeDataAndRedirect() =>
             MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(new Manufacturer() { UserId = TestUser.Identifier, Id = ManufacturerId })
                .WithData(new Product() { Id = ProductId, ManufacturerId = ManufacturerId }))
            .Calling(c => c.Delete(ManufacturerId))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Product>(p => p.Count() == 0)
                .WithSet<Manufacturer>(m => m.Count() == 0))
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");
    }
}
