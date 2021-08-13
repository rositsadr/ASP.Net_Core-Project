using MyTested.AspNetCore.Mvc;
using System.Linq;
using Web.Controllers;
using Web.Models;
using Web.Services.Orders.Models;
using Xunit;
using static Web.WebConstants;
using static Web.Tests.Data.ProductTestData;
using static Web.Tests.Data.UserTestData;
using static Web.Tests.Data.OrderTestData;

namespace Web.Tests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        public void OrderControllerShouldHaveAuthorizedUser() =>
           MyController<OrderController>
           .Instance(controller => controller
               .WithUser(MyTested.AspNetCore.Mvc.TestUser.Identifier, MyTested.AspNetCore.Mvc.TestUser.Username))
           .ShouldHave()
           .Attributes(attributes => attributes
              .RestrictingForAuthorizedRequests());

        [Fact]
        public void OrderDetailsActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithUser()
                .WithPath($"/Order/OrderDetails/{MyTested.AspNetCore.Mvc.TestUser.Identifier}"))
            .To<OrderController>(c => c.OrderDetails(MyTested.AspNetCore.Mvc.TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", 3, 1)]
        public void OrderDetailsControllerShouldReturnViewWithCorrectData(string manufacturerId, string ownerUserId, int count, int quantity) =>
            MyController<OrderController>
            .Instance(controller => controller
                .WithUser(MyTested.AspNetCore.Mvc.TestUser.Identifier, MyTested.AspNetCore.Mvc.TestUser.Username)
                .WithData(GetCartItems(manufacturerId, ownerUserId, count, MyTested.AspNetCore.Mvc.TestUser.Identifier, quantity)))
            .Calling(c => c.OrderDetails(MyTested.AspNetCore.Mvc.TestUser.Identifier))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<OrderServiceModel>()
                .Passing(m => m.Products.Count == count));

        [Fact]
        public void FinalizeOrderActionaRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Order/FinalizeOrder/{MyTested.AspNetCore.Mvc.TestUser.Identifier}")
                .WithUser())
            .To<OrderController>(c => c.FinalizeOrder(MyTested.AspNetCore.Mvc.TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", "3", 4, 1)]
        public void FinalizeOrderActionShouldCrateOrderRemoveCartItemsAndRedirect(string manufacturerId, string ownerUserId, string buyerId, int count, int quantity) => MyController<OrderController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerUserId, count, buyerId, quantity))
                .WithData(UserWithAdditionalData(buyerId, count)))
            .Calling(c=>c.FinalizeOrder(buyerId))
            .ShouldHave()
            .Data(data=>data
                .WithSet<Order>(orders => orders.Any(o=>o.UserId == buyerId))
                .WithSet<Order>(orders => orders.Where(o=>o.UserId == buyerId).FirstOrDefault().OrderProducts.Count == count))
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp.ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("MyOrders", "Users");

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(224)]
        public void DeleteFormArchivesActionRoute(int orderId) =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Order/DeleteFromArchives/{orderId}")
                .WithUser())
            .To<OrderController>(c => c.DeleteFromArchives(orderId));

        [Theory]
        [InlineData("1", 1, 3)]
        [InlineData("user", 5, 7)]
        [InlineData("NextUser", 4, 10)]
        public void DeleteFromArchivesActionShouldDeleteTheOrderWithGivenIdAndRedirectCorrectly(string userId, int orderId, int count) =>
            MyController<OrderController>
            .Instance(controller => controller
                .WithUser(userId, TestUser(userId).UserName)
                .WithData(TestUser(userId))
                .WithData(OrderWithUser(orderId, userId, count)))
            .Calling(c => c.DeleteFromArchives(orderId))
            .ShouldHave()
            .Data(data=>data.
                WithSet<Order>(orders => !orders.Any(o=>o.Id == orderId && o.UserId == userId)))
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("MyOrders", "Users");
    }
}
