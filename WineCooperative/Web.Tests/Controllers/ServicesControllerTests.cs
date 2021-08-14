using System.Linq;
using Xunit;
using MyTested.AspNetCore.Mvc;
using Web.Controllers;
using Web.Models;
using Web.Models.Services;
using Web.Services.Services.Models;
using static Web.WebConstants;
using static Web.Tests.Data.ManufacturerTestData;
using static Web.Tests.Data.ServiceTestData;


namespace Web.Tests.Controllers
{
    public class ServicesControllerTests
    {
        [Fact]
        public void GetAddActionShouldMapRouteAndReturnViewWithData() =>
           MyMvc
           .Pipeline()
           .ShouldMap(request => request
               .WithPath("/Services/Add")
               .WithMethod(HttpMethod.Get)
               .WithUser(TestUser.Identifier, TestUser.Username, MemberRole))
           .To<ServicesController>(c => c.Add())
           .Which()
           .ShouldHave()
           .ActionAttributes(attribute => attribute
               .RestrictingForAuthorizedRequests())
           .AndAlso()
           .ShouldReturn()
           .View(view => view
               .WithModelOfType<ServiceModel>());

        [Fact]
        public void PostAddActionRouteShouldMap() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath("/Services/Add")
                .WithMethod(HttpMethod.Post)
                .WithAntiForgeryToken())
            .To<ServicesController>(c => c.Add(With
                    .Any<ServiceModel>()));

        [Theory]
        [InlineData("TestService", "Somthing for the test.", "https://th.bing.com/th/id/OIP.Y_0vDqlTzmI86k2AGLiNuAHaHa?w=169&h=180&c=7&o=5&dpr=1.5&pid=1.7", 360, true,"TestManufacturer")]
        public void PostAddActionShoulAddProductToListAndRedirectToAll(string serviceName, string description, string imageUrl, decimal price, bool available, string manufacturerId ) =>
           MyController<ServicesController>
           .Instance(controller => controller
               .WithUser(TestUser.Identifier, TestUser.Username, MemberRole)
               .WithData(ManufacturerWithUser(TestUser.Identifier,manufacturerId)))
           .Calling(c => c.Add(new ServiceModel
           {
              Name = serviceName,
              Description = description,
              ImageUrl = imageUrl,
              Price = price,
              Available = available,
              ManufacturerId = manufacturerId
           }))
           .ShouldHave()
           .ActionAttributes(attributes => attributes
               .RestrictingForHttpMethod(HttpMethod.Post)
               .RestrictingForAuthorizedRequests())
           .ValidModelState()
           .Data(data => data
               .WithSet<Service>(services => services
                   .Any(s => s.Name == serviceName && s.Description == description && s.ManufacturerId == manufacturerId && s.Available == available && s.ImageUrl == imageUrl && s.Price == price))
               .WithSet<Service>(services => services.Count() == 1))
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
               .WithPath($"/Services/All/"))
           .To<ServicesController>(c => c.All(With.Any<ServiceSearchPageModel>(), null));

        [Fact]
        public void AllActionRouteMapCorrectlyWithManufacturerId() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Services/All/{ManufacturerId}"))
            .To<ServicesController>(c => c.All(With.Any<ServiceSearchPageModel>(), ManufacturerId));

        [Theory]
        [InlineData(2, "1", "2", 1)]
        [InlineData(6, "1", "2", 2)]
        [InlineData(7, "1", "2", 2)]
        public void AllActionShouldReturnViewWithCorrectData(int count, string manufacturerId, string userId, int currantPage) =>
            MyController<ServicesController>
            .Instance(controller => controller
                .WithData(GetServices(manufacturerId, userId, count)))
            .Calling(c => c.All(new ServiceSearchPageModel { CurrantPage = currantPage }, null))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ServiceSearchPageModel>()
                    .Passing(s => (s.Services.Count() == s.TotalServices - (ServiceSearchPageModel.servicesPerPage * currantPage)) || s.Services.Count() == count || s.Services.Count() == ServiceSearchPageModel.servicesPerPage)
                    .AndAlso()
                    .ShouldPassForThe<ServiceSearchPageModel>(p => p.TotalServices == count));

        [Fact]
        public void GetEditActionRouteWithRouteValueShouldMap() =>
           MyRouting
           .Configuration()
           .ShouldMap(request => request
               .WithPath($"/Services/Edit/{ServiceId}"))
           .To<ServicesController>(c => c.Edit(ServiceId));

        [Theory]
        [InlineData("1", 2)]
        public void GetEditActionShouldReturnViewWithData(string manufacturerId, int count) =>
             MyController<ServicesController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(GetServices(manufacturerId, TestUser.Identifier, count)))
            .Calling(c => c.Edit(count.ToString()))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForAuthorizedRequests())
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ServiceModel>());

        [Fact]
        public void PostEditActionRouteWithRouteValue() =>
            MyRouting
            .Configuration()
            .ShouldMap(request => request
                .WithPath($"/Services/Edit/{ServiceId}")
                .WithMethod(HttpMethod.Post)
                .WithAntiForgeryToken())
            .To<ServicesController>(c => c.Edit(With
                    .Any<ServiceModel>(), ServiceId));

        [Theory]
        [InlineData("ManufacturerTest", 3, "TestService", true, "Test description", "https://th.bing.com/th/id/OIP.Y_0vDqlTzmI86k2AGLiNuAHaHa?w=169&h=180&c=7&o=5&dpr=1.5&pid=1.7", 330)]
        public void PostEditActionShouldChangeTheValueButNotTheCountInDatabase(string manufacturerId, int count, string name, bool available, string description, string imageUrl, decimal price) =>
            MyController<ServicesController>
            .Instance(controller => controller
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole)
                .WithData(GetServices(manufacturerId, TestUser.Identifier, count)))
            .Calling(c => c.Edit(new ServiceModel()
            {
               Name = name,
               Available = available,
               Description = description,
               ImageUrl = imageUrl,
               ManufacturerId=manufacturerId,
               Price = price
            }, count.ToString()))
            .ShouldHave()
            .ActionAttributes(attributes => attributes
                .RestrictingForHttpMethod(HttpMethod.Post)
                .RestrictingForAuthorizedRequests())
            .ValidModelState()
            .Data(data => data
                .WithSet<Service>(s => s.Any(s => s.Id == count.ToString() && s.Name == name && s.ManufacturerId == manufacturerId && s.Available == available && s.ImageUrl == imageUrl && s.Description == description && s.Price == price))
                .WithSet<Service>(s => s.Count() == count))
            .AndAlso()
            .ShouldHave()
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");


        [Theory]
        [InlineData("TestManufacturer", "TestUser", 4)]
        public void DetailsActionShouldMapAndReturnViewWithCorrectData(string manufacturerId, string userId, int count) =>
           MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath($"/Services/Details/{count}"))
            .To<ServicesController>(c => c.Details(count.ToString()))
            .Which(controller => controller
                .WithData(GetServices(manufacturerId, userId, count)))
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<ServiceDetailsIdServiceModel>()
                    .Passing(m => m.Id == count.ToString()));

        [Theory]
        [InlineData("Winary", 3)]
        public void DeletActionShouldMapRemoveServiceFromDataAndRedirect(string manufacturerId, int count) =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath($"/Services/Delete/{count}")
                .WithUser(TestUser.Identifier, TestUser.Username, MemberRole, AdministratorRole))
            .To<ServicesController>(c => c.Delete(count.ToString()))
            .Which(controller => controller
                .WithData(GetServices(manufacturerId, TestUser.Identifier, count)))
            .ShouldHave()
            .Data(data => data
                .WithSet<Service>(s=>s.Count() == count-1)
                .WithSet<Service>(s => !s.Any(s=>s.Id == count.ToString())))
            .TempData(temp => temp
                .ContainingEntryWithKey(SuccessMessageKey))
            .AndAlso()
            .ShouldReturn()
            .RedirectToAction("All");

    }
}
