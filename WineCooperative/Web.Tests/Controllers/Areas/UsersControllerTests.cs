using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using MyTested.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Web.Data.Models;
using Web.Areas.Admin.Controllers;
using Web.Services.Users.Models;
using static Web.WebConstants;
using static Web.Areas.AreaConstants;
using static Web.Tests.Data.UserTestData;
using static Web.Tests.Data.ManufacturerTestData;
using static Web.Tests.Data.ProductTestData;
using static Web.Tests.Data.ServiceTestData;
using Web.Services.Manufacturers.Models;

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
        [InlineData("TestUserId", "TestMemberRole")]
        public void ApproveMemberActionShouldGiveRoleToTheUserAndShouldRedirect(string userId, string roleId) =>
            MyController<UsersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, AdministratorRole)
                .WithData(CustomeTestUser(userId), GetMemberRole(roleId))
                .WithMemoryCache(cache => cache
                    .WithEntry(entry => entry
                       .WithKey(MemberRole)
                       .WithValue(With.Any<List<UserInfoServiceModel>>()))
                    .WithEntry(entry => entry
                        .WithKey(applyedCacheKey)
                        .WithValue(With.Any<List<UserInfoServiceModel>>()))))
            .Calling(c => c.ApproveMember(userId))
            .ShouldHave()
            .MemoryCache(cache => cache
                .ContainingEntry(entry => entry
                    .WithKey(membersCacheKey)
                    .WithValue(null))
                .ContainingEntry(entry => entry
                    .WithKey(applyedCacheKey)
                    .WithValue(null)))
            .Data(data => data
                .WithSet<User>(users => Assert.False(users.Find(userId).Applyed))
                .WithSet<IdentityUserRole<string>>( usersRoles => usersRoles.Find(userId,roleId)))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("ApplyedUsers");

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
            .ShouldMap("/Admin/Users/AllMembers")
            .To<UsersController>(c=>c.AllMembers());

        [Theory]
        [InlineData("TestRoleId", "TestUserId")]
        public void AllMembersActionShouldReturnViewWithCorrectData(string roleId, string userId) =>
            MyController<UsersController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier,TestUser.Username,AdministratorRole)
                .WithData(GetMemberRole(roleId),CustomeTestUser(userId), GetUserRole(userId,roleId)))
            .Calling(c=>c.AllMembers())
            .ShouldHave()
            .MemoryCache(cache => cache
                .ContainingEntry(entry => entry
                    .WithKey(membersCacheKey)
                    .WithValueOfType<List<UserInfoServiceModel>>()
                    .Passing(c=> Assert.Equal(Assert.Single(c).Id, userId))))
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<UserInfoServiceModel>>()
                .Passing(m => Assert.Equal(Assert.Single(m).Id,userId)));

        [Theory]
        [InlineData("TestUserId", "TestRoleId", "TestManufacturerId", "TestServiceId", "TestProductId")]
        public void RemoveMemberActionRouteShouldMapCorrectlyAndActionShouldRemoveUserFromRoleMemberAndRedirect(string userId, string roleId, string manufacturerId, string serviceId, string productId) =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath($"/Admin/Users/RemoveMember/{userId}")
                .WithUser(TestUser.Identifier,TestUser.Username,AdministratorRole))
            .To<UsersController>(c=>c.RemoveMember(userId))
            .Which(controller => controller
                .WithData(GetMemberRole(roleId), CustomeTestUser(userId), GetUserRole(userId, roleId), GetManufacturerWithUser(userId,manufacturerId), GetServiceWithManufacturer(serviceId,manufacturerId),GetProductWithManufacturer(productId,manufacturerId))
                .WithMemoryCache(cache => cache
                    .WithEntry(entry => entry
                        .WithKey(manufacturersCacheKey)
                        .WithValue(With.Any<IEnumerable<ManufacturerServiceModel>>()))
                    .WithEntry(entry => entry
                        .WithKey(membersCacheKey)
                        .WithValue(With.Any<List<UserInfoServiceModel>>()))))
            .ShouldHave()
            .MemoryCache(cache => cache
                .ContainingEntry(entry => entry
                    .WithKey(manufacturersCacheKey)
                    .WithValue(null))
                .ContainingEntry(entry => entry 
                    .WithKey(membersCacheKey)
                    .WithValue(null)))
            .AndAlso()
            .ShouldHave()
            .Data(data => data
                .WithSet<Manufacturer>(ma => Assert.False(Assert.Single(ma).IsFunctional))
                .WithSet<Product>(p => Assert.False(Assert.Single(p).InStock))
                .WithSet<Service>(s => Assert.False(Assert.Single(s).Available))
                .WithSet<IdentityUserRole<string>>(ur=> Assert.Null(ur.Find(userId,roleId))))
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("AllMembers");
    }
}
