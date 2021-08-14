using System.Collections.Generic;
using System.Linq;
using MyTested.AspNetCore.Mvc;
using Web.Models;
using Web.Controllers;
using Web.Models.Products;
using Web.Services.Products.Models;
using Xunit;
using static Web.WebConstants;
using static Web.Tests.Data.UserTestData;
using static Web.Tests.Data.ManufacturerTestData;
using static Web.Tests.Data.ProductTestData;


namespace Web.Tests.Controllers
{
    public class ProductsControllerTests
    {
        [Fact]
        public void GetAddActionShouldMapRouteAndReturnViewWithData() =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath("/Products/Add")
                .WithMethod(HttpMethod.Get)
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole))
            .To<ProductsController>(c => c.Add())
            .Which()
            .ShouldHave()
            .ActionAttributes(attribute => attribute
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ProductModel>());

        [Fact]
        public void PostAddActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Products/Add")
                .WithMethod(HttpMethod.Post)
                .WithAntiForgeryToken())
            .To<ProductsController>(c => c.Add(With
                    .Any<ProductModel>()));

        [Theory]
        [InlineData("TestProduct", 10, "https://th.bing.com/th/id/R.1b4f1c77b96e722050648ab162ace69d?rik=uC2oIjMEbBG5ng&pid=ImgRaw&r=0", 2000, true, "1", "1", 3, 2, 4, 1)]
        public void PostAddActionShoulAddProductToListAndRedirectToAll(string productName, decimal productPrice, string productImageUrl, int productManufacturerYear, bool inStock, string userId, string manufacturerId, int tasteId, int colorId, int grapeId, int wineAreaId) =>
            MyController<ProductsController>
            .Instance(controller => controller
                .WithUser(userId, "testUser", MemberRole)
                .WithData(CustomeTestUser(userId), ManufacturerWithUser(userId, manufacturerId), TestTaste(tasteId), TestColor(colorId), TestGrape(grapeId), TestWineArea(wineAreaId)))
            .Calling(c => c.Add(new ProductModel
            {
                Name = productName,
                Price = productPrice,
                ImageUrl = productImageUrl,
                ManufactureYear = productManufacturerYear,
                InStock = inStock,
                ManufacturerId = manufacturerId,
                TasteId = tasteId,
                ColorId = colorId,
                WineAreaId = wineAreaId,
                GrapeVarieties = new List<int>() { grapeId }
            }))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .Data(data => data
                .WithSet<Product>(products => products
                    .Any(p => p.Name == productName && p.ManufacturerId == manufacturerId))
                .WithSet<Product>(products => products.Count() == 1))
            .AndAlso()
            .ShouldHave()
            .TempData(tempData => tempData
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

        [Fact]
        public void AllActionRouteMapCorrectly() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Products/All/"))
            .To<ProductsController>(c => c.All(With.Any<ProductSearchPageModel>(),null));

        [Fact]
        public void AllActionRouteMapCorrectlyWithManufacturerId() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Products/All/{ManufacturerId}"))
            .To<ProductsController>(c => c.All(With.Any<ProductSearchPageModel>(), ManufacturerId));

        [Theory]
        [InlineData(2,"1","2",1, 1,2,3)]
        [InlineData(6, "1", "2", 2, 4,5,6)]
        [InlineData(7, "1", "2", 2,7,8,9)]
        public void AllActionShouldReturnViewWithCorrectData(int count, string manufacturerId, string userId, int currantPage, int tasteId, int colorId, int wineAreId) =>
            MyController<ProductsController>
            .Instance(controller => controller
                .WithData(GetProducts(manufacturerId, userId, count, tasteId, wineAreId, colorId)))
            .Calling(c => c.All(new ProductSearchPageModel{ CurrantPage = currantPage }, null))      
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ProductSearchPageModel>()
                    .Passing(p =>(p.Products.Count() == p.TotalProducts-(ProductSearchPageModel.productsPerPage*currantPage)) || p.Products.Count() == count || p.Products.Count() == ProductSearchPageModel.productsPerPage)
                    .AndAlso()
                    .ShouldPassForThe<ProductSearchPageModel>(p=>p.TotalProducts == count));

        [Fact]
        public void GetEditActionRouteWithRouteValue() =>
           MyRouting
           .Configuration()
           .ShouldMap(request => request
               .WithPath($"/Products/Edit/{ProductId}"))
           .To<ProductsController>(c => c.Edit(ProductId));

        [Theory]
        [InlineData("1",2, 1, 2, 3)]
        public void GetEditActionShouldReturnViewWithData(string manufacturerId, int count, int tasteId, int colorId, int wineAreId) =>
             MyController<ProductsController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(GetProducts(manufacturerId, TestUser.Identifier, count, tasteId, wineAreId, colorId)))
            .Calling(c => c.Edit(count.ToString()))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ProductModel>());

        [Fact]
        public void PostEditActionRouteWithRouteValue() => 
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Products/Edit/{ProductId}")
                .WithMethod(HttpMethod.Post)
                .WithAntiForgeryToken())
            .To<ProductsController>(c => c.Edit(With
                    .Any<ProductModel>(), ProductId));

        [Theory]
        [InlineData("Winary", 3, "TestProduct", 2, 3, 6, 5, true, "https://th.bing.com/th/id/R.1b4f1c77b96e722050648ab162ace69d?rik=uC2oIjMEbBG5ng&pid=ImgRaw&r=0", 2020, 12.60,1,2,3)]
        public void PostEditActionShouldChangeTheValueButNotTheCountInDatabase(string manufacturerId, int count, string name, int colorTestId, int tasteTestId, int wineAreaTestId, int grapeId, bool inStock, string imageUrl, int year, decimal price, int tasteId, int wineareId, int colorId) =>
            MyController<ProductsController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(CustomeTestUser(TestUser.Identifier), TestTaste(tasteTestId), TestColor(colorTestId), TestGrape(grapeId), TestWineArea(wineAreaTestId))
            .WithData(GetProducts(manufacturerId, TestUser.Identifier, count, tasteId,  wineareId, colorId)))
            .Calling(c => c.Edit(new ProductModel()
            {
                Name = name,
                ColorId = colorTestId,
                TasteId = tasteTestId,
                WineAreaId = wineAreaTestId,
                GrapeVarieties = new List<int> { grapeId },
                InStock = inStock,
                ImageUrl = imageUrl,
                ManufactureYear = year,
                Price = price,
                ManufacturerId = manufacturerId
            }, count.ToString()))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .Data(data => data
                .WithSet<Product>(p => p.Any(p => p.Id == count.ToString() && p.Name == name && p.ManufacturerId == manufacturerId && p.InStock == inStock && p.ImageUrl == imageUrl && p.ManufactureYear == year && p.ColorId == colorTestId && p.TasteId == tasteTestId && p.WineAreaId == wineAreaTestId && p.Price == price && p.GrapeVarieties.Any(gv => gv.GrapeVarietyId == grapeId) && p.GrapeVarieties.Count == 1))
                .WithSet<Product>(p => p.Count() == count))
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

        [Theory]
        [InlineData("Winary", 3, "TestUser",1,2,3)]
        [InlineData("Winary!", 8, "TestUser3",3,4,5)]
        [InlineData("Winary@", 5, "TestUser2",6,7,8)]
        public void DetailsActionShouldMapAndReturnViewWithCorrectData(string manufacturerId, int count, string ownerId, int tasteId, int colorId, int wineAreId) =>
           MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath($"/Products/Details/{count}"))
            .To<ProductsController>(c=>c.Details(count.ToString()))
            .Which(controller => controller
                .WithData(GetProducts(manufacturerId, ownerId, count, tasteId, wineAreId, colorId)))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ProductDetailsServiceModel>()
                    .Passing(m=>m.Id == count.ToString()));

        [Theory]
        [InlineData("Winary", 3,1,2,3)]
        public void DeletActionShouldRemoveProductFromDataAndRedirect(string manufacturerId, int count, int tasteId, int colorId, int wineAreId) =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath($"/Products/Delete/{count}")
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole))
            .To<ProductsController>(c=>c.Delete(count.ToString()))
            .Which(controller => controller
                .WithData(GetProducts(manufacturerId, TestUser.Identifier, count, tasteId, wineAreId, colorId)))
            .ShouldHave()
            .Data(data => data
                .WithSet<Product>(p => p.Count() == count - 1)
                .WithSet<Product>(p => !p.Any(p => p.Id == count.ToString())))
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

    }
}
