using MyTested.AspNetCore.Mvc;
using Web.Controllers;
using Web.Services.Products.Models;
using Web.Services.Services.Models;
using Web.Services.Manufacturers.Models;
using Web.Services.Orders.Models;
using System.Collections.Generic;
using Xunit;
using static Web.WebConstants;
using static Web.Areas.AreaConstants;
using static Web.Tests.Data.ProductTestData;
using static Web.Tests.Data.ServiceTestData;
using static Web.Tests.Data.ManufacturerTestData;
using static Web.Tests.Data.OrderTestData;
using static Web.Tests.Data.UserTestData;
using Web.Services.Users.Models;

namespace Web.Tests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public void UsersControllerShouldHaveAuthorizedUser() =>
            MyController<UsersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username))
            .ShouldHave()
            .Attributes(attributes => attributes
               .RestrictingForAuthorizedRequests());

        [Theory]
        [InlineData("TestManufacturer", 3, 1, 2, 3)]
        public void MyProductsShouldMapCorrectlyAndShouldReturnViewWithCorrectData(string manufacturerId, int count, int tasteId, int wineAreaId, int colorId) =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath("/Users/MyProducts")
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole))
            .To<UsersController>(c => c.MyProducts())
            .Which(controller => controller
                .WithData(GetProducts(manufacturerId,TestUser.Identifier, count, tasteId, wineAreaId,colorId)))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<ProductDetailsServiceModel>>()
                .Passing(m => m.Count == count));

        [Theory]
        [InlineData("TestManufacturer", 3)]
        public void MyServicesShouldMapCorrectlyAndShouldReturnViewWithCorrectData(string manufacturerId, int count) =>
           MyMvc
           .Pipeline()
           .ShouldMap(request => request
               .WithPath("/Users/MyServices")
               .WithUser(TestUser.Identifier, TestUser.Username, MemberRole))
           .To<UsersController>(c => c.MyServices())
           .Which(controller => controller
               .WithData(GetServices(manufacturerId, TestUser.Identifier, count)))
           .ShouldReturn()
           .View(view => view
               .WithModelOfType<List<ServiceDetailsServiceModel>>()
               .Passing(m => m.Count == count));

        [Theory]
        [InlineData( 3)]
        public void MyManufacturersShouldMapCorrectlyAndShouldReturnViewWithCorrectData( int count) =>
           MyMvc
           .Pipeline()
           .ShouldMap(request => request
               .WithPath("/Users/MyManufacturers")
               .WithUser(TestUser.Identifier, TestUser.Username, MemberRole))
           .To<UsersController>(c => c.MyManufacturers())
           .Which(controller => controller
               .WithData(GetManufacturers(count)))
           .ShouldReturn()
           .View(view => view
               .WithModelOfType<List<ManufacturerServiceModel>>()
               .Passing(m => m.Count == count));

        [Theory]
        [InlineData(3, 5)]
        public void MyOrdersShouldMapCorrectlyAndShouldReturnViewWithCorrectData(int productsCount, int orderId) =>
          MyMvc
          .Pipeline()
          .ShouldMap(request => request
              .WithPath($"/Users/MyOrders/{TestUser.Identifier}")
              .WithUser(TestUser.Identifier, TestUser.Username, MemberRole))
          .To<UsersController>(c => c.MyOrders(TestUser.Identifier))
          .Which(controller => controller
              .WithData(OrderWithUserAndProduct(productsCount, orderId, TestUser.Identifier)))
          .ShouldReturn()
          .View(view => view
              .WithModelOfType<List<OrderServiceModel>>()
              .Passing(m => m.Count == 1));

        [Fact]
        public void BecomeMemberActionShouldMapCorrectAndReturnView() =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath("/Users/BecomeMember")
                .WithUser())
            .To<UsersController>(c => c.BecomeMember());

        [Fact]
        public void ApplyActionRouteShouldMap() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Users/Apply/{TestUser.Identifier}")
                .WithUser())
            .To<UsersController>(c=>c.Apply(TestUser.Identifier));

        [Theory]
        [InlineData("TestUser")]
        public void ApplyActionShouldMapCorrectly(string userId) =>
            MyController<UsersController>
            .Instance(controller => controller
                .WithUser(userId, "testUser")
                .WithData(CustomeTestUser(userId))
                .WithMemoryCache(cache => cache
                    .WithEntry(entry => entry
                        .WithKey(applyedCacheKey)
                        .WithValue(With.Any<List<UserInfoServiceModel>>()))))
            .Calling(c => c.Apply(userId))
            .ShouldHave()
            .MemoryCache(cache => cache
                .ContainingEntry(entity => entity
                    .WithKey(applyedCacheKey)
                    .WithValue(null)))
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey));
            //.AndAlso()
            //.ShouldReturn()
            //.Redirect(redirect => redirect
            //    .ToPage("/Account/Manage/Index")
            //    .ContainingRouteValue(new { area = "Identity" }));
    }
}
