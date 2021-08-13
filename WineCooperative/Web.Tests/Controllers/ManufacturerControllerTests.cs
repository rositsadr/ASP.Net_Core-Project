using System.Linq;
using System.Collections.Generic;
using Web.Controllers;
using Web.Models;
using Web.Models.Manufacturers;
using Web.Services.Manufacturers.Models;
using MyTested.AspNetCore.Mvc;
using Xunit;
using static Web.WebConstants;
using static Web.Areas.AreaConstants;
using static Web.Tests.Data.ManufacturerTestData;
using static Web.Tests.Data.ProductTestData;


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
                .WithMethod(HttpMethod.Post)
                .WithAntiForgeryToken())
            .To<ManufacturersController>(c=>c.Add(With
                    .Any<ManufacturerModel>()));

        [Theory]
        [InlineData("Winary", "00359876543", "winery@abv.bg", true, "Kovachev", "3124", "Spasovo",3)]
        public void PostAddActionShouldAddManufacturerToDataIfModelStateIsValid(string name, string phoneNumber, string email, bool isFunctional, string street, string zipCode, string townName, int count) =>
            MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(roles: MemberRole)
                .WithData(GetManufacturers(count))
                .WithMemoryCache(cache => cache
                    .WithEntry(entity => entity
                        .WithKey(membersCacheKey)
                        .WithValue(With.Any<List<ManufacturerServiceModel>>()))))
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
                    .Any(m => m.Name == name && m.UserId == TestUser.Identifier))
                .WithSet<Manufacturer>(m=>m.Count() == count+1))
            .AndAlso()
            .ShouldHave()
            .NoMemoryCache()
            .AndAlso()
            .ShouldHave() 
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

        [Theory]
        [InlineData(2,2)]
        public void AllActionShouldReturnView(int count, int expectedCount) =>
            MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(GetManufacturers(count)))
            .Calling(c => c.All())
            .ShouldHave()
            .MemoryCache(cache => cache
                    .ContainingEntry(entry => entry
                        .WithKey(manufacturersCacheKey)
                        .WithValueOfType<List<ManufacturerServiceModel>>()))
                .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<ManufacturerServiceModel>>()
                    .Passing(m=>m.Count() == expectedCount));

        [Fact]
        public void GetEditActionRouteWithRouteValue() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Manufacturers/Edit/{ManufacturerId}"))
            .To<ManufacturersController>(c => c.Edit(ManufacturerId));

        [Theory]
        [InlineData(2)]
        public void GetEditActionShouldReturnViewWithData(int count) =>
             MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(user=> user.InRole(MemberRole))
                .WithData(GetManufacturers(count)))
            .Calling(c => c.Edit(count.ToString()))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ManufacturerModel>());

        [Fact]
        public void PostEditActionRouteWithRuteValue()=>
              MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Manufacturers/Edit/{ManufacturerId}")
                .WithMethod(HttpMethod.Post)
                .WithAntiForgeryToken())
            .To<ManufacturersController>(c => c.Edit(With
                    .Any<ManufacturerModel>(), ManufacturerId));

        [Theory]
        [InlineData("Winary", "00359876543", "winery@abv.bg", true, "Kovachev", "3124", "Spasovo",3)]
        public void PostEditActionShouldChangeTheValueButNotTheCountInDatabase (string name, string phoneNumber, string email, bool isFunctional, string street, string zipCode, string townName, int count) =>
            MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(GetManufacturers(count))
                .WithMemoryCache(cache => cache
                    .WithEntry(entity => entity
                        .WithKey(membersCacheKey)
                        .WithValue(With.Any<List<ManufacturerServiceModel>>()))))
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
            }, count.ToString()))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .Data(data=>data
                .WithSet<Manufacturer>(m=>m.Any(m=>m.Id == count.ToString() && m.Name == name && m.UserId == TestUser.Identifier && m.PhoneNumber == phoneNumber && m.IsFunctional == isFunctional && m.Email == email && m.Address.Street == street && m.Address.Town.Name == townName && m.Address.ZipCode == zipCode))
                .WithSet<Manufacturer>(m=>m.Count() == count))
            .AndAlso()
            .ShouldHave()
            .NoMemoryCache()
            .AndAlso()
            .ShouldHave()
            .TempData(temp=>temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

        [Fact]
        public void DeleteActionRouteWithRouteValues() =>
            MyPipeline
            .Configuration()
            .ShouldMap(request=>request
                .WithPath($"/Manufacturers/Delete/{ManufacturerId}")
                .WithUser(TestUser.Identifier,TestUser.Username, MemberRole,AdministratorRole))
            .To<ManufacturersController>(c => c.Delete(ManufacturerId));

        [Fact]
        public void DeleteActionShouldChangeDataAndRedirect() =>
             MyController<ManufacturersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(new Manufacturer() { UserId = TestUser.Identifier, Id = ManufacturerId })
                .WithData(new Product() { Id = ProductId, ManufacturerId = ManufacturerId })
                .WithMemoryCache(cache => cache
                    .WithEntry(entity => entity
                        .WithKey(membersCacheKey)
                        .WithValue(With.Any<List<ManufacturerServiceModel>>()))))
            .Calling(c => c.Delete(ManufacturerId))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .Data(data => data
                .WithSet<Product>(p => p.Count() == 0)
                .WithSet<Manufacturer>(m => m.Count() == 0))
            .AndAlso()
            .ShouldHave()
            .NoMemoryCache()
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");
    }
}
