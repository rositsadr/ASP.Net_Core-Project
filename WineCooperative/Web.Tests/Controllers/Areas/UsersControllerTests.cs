using Xunit;
using MyTested.AspNetCore.Mvc;
using Web.Areas.Admin.Controllers;
using static Web.WebConstants;
using static Web.Tests.Data.UserTestData;
using static Web.Areas.AreaConstants;
using Web.Data.Models;
using System.Linq;
using Web.Services.Users.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Web.Tests.Controllers.Areas
{
    public class UsersControllerTests
    {
        [Fact]
        public void UsersControllerHasAuthorizeAttributeWithRoleAdministratorAndAreaAdmin() =>
            MyController<UsersController>
            .ShouldHave()
            .Attributes(attribute => attribute
                .RestrictingForAuthorizedRequests(withAllowedRoles: AdministratorRole)
                .SpecifyingArea("Admin"));

        [Fact]
        public void ApplayedUsersActionShouldMapCorrectly() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Admin/Users/ApplyedUsers")
                .WithUser(TestUser.Identifier, TestUser.Username, AdministratorRole))
            .To<UsersController>(c => c.ApplyedUsers());

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(15)]
        public void ApplyedUsersActionShouldReturnViewWithCorrectData(int count) =>
            MyController<UsersController>
            .Instance(controller => controller
                .WithData(GetApplyedUsers(count)))
            .Calling(c => c.ApplyedUsers())
            .ShouldHave()
            .MemoryCache(cache => cache
                    .ContainingEntry(entry => entry
                        .WithKey(applyedCacheKey)
                        .WithValueOfType<List<UserInfoServiceModel>>()
                        .Passing(c => Assert.Equal(c.Count(), count))))
                .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<UserInfoServiceModel>>()
                    .Passing(m => Assert.Equal(m.Count(), count)));

        [Theory]
        [InlineData("TestUserId")]
        public void ApproveMemberActionShouldMapCorrectly(string userId) =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Admin/Users/ApproveMember/{userId}")
                .WithUser(TestUser.Identifier, TestUser.Username, AdministratorRole))
            .To<UsersController>(c => c.ApproveMember(userId));

        [Theory]
        [InlineData("TestUserId")]
        public void DeclineMemberActionShouldMapCorrectlyDeleteApplicationClearCacheDataAndRedirect(string userId) =>
             MyMvc
             .Pipeline()
             .ShouldMap(request => request
                 .WithPath($"/Admin/Users/DeclineMember/{userId}")
                 .WithUser(TestUser.Identifier, TestUser.Username, AdministratorRole))
             .To<UsersController>(c => c.DeclineMember(userId))
             .Which(controller => controller
                 .WithData(CustomeTestApplyedUser(userId))
                 .WithMemoryCache(cache => cache
                     .WithEntry(entity => entity
                         .WithKey(applyedCacheKey)
                         .WithValue(With.Any<List<UserInfoServiceModel>>()))))
            .ShouldHave()
            .MemoryCache(cache => cache
                .ContainingEntry(entity => entity
                         .WithKey(applyedCacheKey)
                         .WithValue(null)))
            .Data(data => data
                .WithSet<User>(u=> Assert.False(u.Find(userId).Applyed)))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("ApplyedUsers");

        [Fact]
        public void AllMembersActionShouldMapCorrectly() =>
            MyRouting
            .Configuration()
            .ShouldMap("/Admin/Users/AllMembers");

           
    }
}
