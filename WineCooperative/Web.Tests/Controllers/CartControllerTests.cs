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
                .WithUser(TestUser.Identifier, TestUser.Username))
            .ShouldHave()
            .Attributes(attributes => attributes
               .RestrictingForAuthorizedRequests());

        [Fact]
        public void MyCartActionRoute() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
               .WithPath("/Cart/MyCart")
               .WithQuery("userId", TestUser.Identifier)
               .WithUser(TestUser.Identifier, TestUser.Username))
            .To<CartController>(c => c.MyCart(TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", 3,1, 1,2,3)]
        public void MyCartControllerShouldHaveAuthorizedUserAndReturnViewWithCorrectData(string manufacturerId, string ownerUserId, int count, int quantity, int tasteId, int wineAreId, int colorId) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username)
                .WithData(GetCartItems(manufacturerId, ownerUserId, count,TestUser.Identifier, quantity, tasteId, wineAreId, colorId)))
            .Calling(c => c.MyCart(TestUser.Identifier))
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
                .WithQuery("userId", TestUser.Identifier))
            .To<CartController>(c => c.AddToCart(ProductId, TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", "3", 4, 1, 2, 3)]
        public void AddToCartActionShouldAddProductToCartAndRedirectToAll(string manufacturerId, string ownerId, string buyerId, int count, int tasteId, int colorId, int wineAreId) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId,"testUser")
                .WithData(GetProducts(manufacturerId, ownerId, count, tasteId, wineAreId, colorId))
                .WithData(CustomeTestUser(buyerId)))
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
                .WithQuery("userId", TestUser.Identifier))
            .To<CartController>(c => c.Add(ProductId, TestUser.Identifier));


        [Theory]
        [InlineData("1", "2", "3", 4, 1, 1,2,3)]
        public void AddActionShouldIncreaseQuantityOfCartItem(string manufacturerId, string ownerId, string buyerId, int count, int quantity, int tasteId, int wineareId, int colorId) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity, tasteId, wineareId, colorId))
                .WithData(CustomeTestUser(buyerId)))
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
               .WithQuery("userId", TestUser.Identifier))
           .To<CartController>(c => c.Remove(ProductId, TestUser.Identifier));


        [Theory]
        [InlineData("1", "2", "3", 4, 2, 1, 2, 3)]
        public void RemoveActionShouldDecreaseQuantityOfCartItem(string manufacturerId, string ownerId, string buyerId, int count, int quantity, int tasteId, int wineareId, int colorId) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity,  tasteId, wineareId, colorId))
                .WithData(CustomeTestUser(buyerId)))
            .Calling(c => c.Remove(count.ToString(), buyerId))
            .ShouldHave()
            .Data(data => data
                .WithSet<CartItem>(items => items.Find(count.ToString(), buyerId).Quantity == quantity - 1))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("MyCart");


        [Theory]
        [InlineData("1", "2", "3", 4, 1, 1,2,3)]
        public void RemoveActionShouldNotRemoveIfQuantityIsLessThenTwo(string manufacturerId, string ownerId, string buyerId, int count, int quantity, int tasteId, int colorId, int wineAreId) =>
            MyController<CartController>
            .Instance(controller => controller
                .WithUser(buyerId, "testUser")
                .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity, tasteId, wineAreId, colorId))
                .WithData(CustomeTestUser(buyerId)))
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
               .WithQuery("userId", TestUser.Identifier))
            .To<CartController>(c=> c.Delete(ProductId, TestUser.Identifier));

        [Theory]
        [InlineData("1", "2", "3", 4, 2, 1, 2, 3)]
        public void DeleteActionShouldRemoveProductFromCartItem(string manufacturerId, string ownerId, string buyerId, int count, int quantity, int tasteId, int wineareId, int colorId) =>
           MyController<CartController>
           .Instance(controller => controller
               .WithUser(buyerId, "testUser")
               .WithData(GetCartItems(manufacturerId, ownerId, count, buyerId, quantity, tasteId, wineareId, colorId))
               .WithData(CustomeTestUser(buyerId)))
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
