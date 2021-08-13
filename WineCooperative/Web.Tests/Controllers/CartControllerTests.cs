using MyTested.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web.Controllers;
using Web.Data.Models;
using Web.Services.Cart.Models;
using Xunit;
using static Web.WebConstants;
using static Web.Tests.Data.ProductTestData;
using static Web.Tests.Data.UserTestData;

namespace Web.Tests.Controllers
{
    public class CartControllerTests
    {
        [Fact]
        public void CartControllerShouldHaveAuthorizedUser() =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(MyTested.AspNetCore.Mvc.TestUser.Identifier, MyTested.AspNetCore.Mvc.TestUser.Username))
            .ShouldHave()
            .Attributes(attributes => attributes
               .RestrictingForAuthorizedRequests());

        [Fact]
        public void MyCartActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
               .WithPath("/Cart/MyCart")
               .WithQuery("userId", MyTested.AspNetCore.Mvc.TestUser.Identifier)
               .WithUser(MyTested.AspNetCore.Mvc.TestUser.Identifier, MyTested.AspNetCore.Mvc.TestUser.Username))
            .To<CartController>(c => c.MyCart(MyTested.AspNetCore.Mvc.TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", 3,1)]
        public void MyCartControllerShouldHaveAuthorizedUserAndReturnViewWithCorrectData(string manufacturerId, string ownerUserId, int count, int quantity) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(MyTested.AspNetCore.Mvc.TestUser.Identifier, MyTested.AspNetCore.Mvc.TestUser.Username)
                .WithData(GetCartItems(manufacturerId, ownerUserId, count, MyTested.AspNetCore.Mvc.TestUser.Identifier, quantity)))
            .Calling(c => c.MyCart(MyTested.AspNetCore.Mvc.TestUser.Identifier))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<CartItemViewServiceModel>>()
                .Passing(m => m.Count == count));

        [Fact]
        public void AddToCartActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Cart/AddToCart")
                .WithUser()
                .WithQuery("productId", ProductId)
                .WithQuery("userId", MyTested.AspNetCore.Mvc.TestUser.Identifier))
            .To<CartController>(c => c.AddToCart(ProductId, MyTested.AspNetCore.Mvc.TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", "3", 4)]
        public void AddToCartActionShouldAddProductToCartAndRedirectToAll(string manufacturerId, string ownerId, string buyerId, int count) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId,"testUser")
                .WithData(GetProducts(manufacturerId, ownerId, count))
                .WithData(TestUser(buyerId)))
            .Calling(c => c.AddToCart(count.ToString(), buyerId))
            .ShouldHave()
            .Data(data => data
                .WithSet<CartItem>(items => items.Find(count.ToString(), buyerId)))
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All","Products");

        [Fact]
        public void AddActionRoute()=>
             MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Cart/Add")
                .WithUser()
                .WithQuery("productId", ProductId)
                .WithQuery("userId", MyTested.AspNetCore.Mvc.TestUser.Identifier))
            .To<CartController>(c => c.Add(ProductId, MyTested.AspNetCore.Mvc.TestUser.Identifier));


        [Theory]
        [InlineData("1", "2", "3", 4, 1)]
        public void AddActionShouldIncreaseQuantityOfCartItem(string manufacturerId, string ownerId, string buyerId, int count, int quantity) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity))
                .WithData(TestUser(buyerId)))
            .Calling(c => c.Add(count.ToString(), buyerId))
            .ShouldHave()
            .Data(data => data
                .WithSet<CartItem>(items => items.Find(count.ToString(), buyerId).Quantity == quantity + 1))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("MyCart");

        [Fact]
        public void RemoveActionRoute() =>
            MyRouting
           .Configuration()
           .ShouldMap(request => request
               .WithPath("/Cart/Remove")
               .WithUser()
               .WithQuery("productId", ProductId)
               .WithQuery("userId", MyTested.AspNetCore.Mvc.TestUser.Identifier))
           .To<CartController>(c => c.Remove(ProductId, MyTested.AspNetCore.Mvc.TestUser.Identifier));


        [Theory]
        [InlineData("1", "2", "3", 4, 2)]
        public void RemoveActionShouldDecreaseQuantityOfCartItem(string manufacturerId, string ownerId, string buyerId, int count, int quantity) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity))
                .WithData(TestUser(buyerId)))
            .Calling(c => c.Remove(count.ToString(), buyerId))
            .ShouldHave()
            .Data(data => data
                .WithSet<CartItem>(items => items.Find(count.ToString(), buyerId).Quantity == quantity - 1))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("MyCart");


        [Theory]
        [InlineData("1", "2", "3", 4, 1)]
        public void RemoveActionShouldNotRemoveIfQuantityIsLessThenTwo(string manufacturerId, string ownerId, string buyerId, int count, int quantity) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity))
                .WithData(TestUser(buyerId)))
            .Calling(c => c.Remove(count.ToString(), buyerId))
            .ShouldHave()
            .Data(data => data
                .WithSet<CartItem>(items => items.Find(count.ToString(), buyerId).Quantity == quantity))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("MyCart");

        [Fact]
        public void DeleteActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Cart/Delete")
                .WithUser()
                .WithQuery("productId", ProductId)
               .WithQuery("userId", MyTested.AspNetCore.Mvc.TestUser.Identifier))
            .To<CartController>(c=> c.Delete(ProductId, MyTested.AspNetCore.Mvc.TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", "3", 4, 2)]
        public void DeleteActionShouldRemoveProductFromCartItem(string manufacturerId, string ownerId, string buyerId, int count, int quantity) =>
           MyController<CartController>
           .Instance(controller => controller
               .WithUser(buyerId, "testUser")
               .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity))
               .WithData(TestUser(buyerId)))
           .Calling(c => c.Delete(count.ToString(), buyerId))
           .ShouldHave()
           .Data(data => data
               .WithSet<CartItem>(items => items.Find(count.ToString(), buyerId) == null)
           .WithSet<CartItem>(items=>items.Count() == count-1))
           .AndAlso()
           .ShouldReturn()
           .RedirectToAction("MyCart");
    }
}
