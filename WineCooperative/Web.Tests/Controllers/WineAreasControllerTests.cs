using MyTested.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web.Controllers;
using Web.Data.Models;
using Web.Services.WineAreas.Models;
using Xunit;
using static Web.Tests.Data.WineAreaTestData;

namespace Web.Tests.Controllers
{
    public class WineAreasControllerTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(7)]
        public void MapActionRouteShouldMapAndActionShouldReturnViewWithCorrectData(int count) =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath("/WineAreas/Map"))
            .To<WineAreasController>(c => c.Map())
            .Which(controller => controller
                .WithData(GetWineAreas(count)))
            .ShouldHave()
            .Data(data=>data
                .WithSet<WineArea>(wineAreas=> Assert.Equal(count,wineAreas.Count())))
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<List<WineAreaDisplayServiceModel>>()
                .Passing(m => Assert.Equal(m.Count, count)));

        [Theory]
        [InlineData(4, 2)]
        public void DetailsActionRouteShouldMapAndActionShouldReturnViewWithCorrectData(int wineAreaId, int productCount) =>
            MyMvc
            .Pipeline()
            .ShouldMap(request => request
                .WithPath($"/WineAreas/Details")
                .WithQuery("wineAreaId", wineAreaId.ToString()))
            .To<WineAreasController>(c => c.Details(wineAreaId))
            .Which(controller => controller
                .WithData(GetWineAreaWithProductsAndFullData(wineAreaId, productCount)))
            .ShouldHave()
            .Data(data => data
                .WithSet<WineArea>(wineAreas => wineAreas.Find(wineAreaId)))
            .AndAlso()
            .ShouldReturn()
            .View(view => view
                .WithModelOfType<WineAreaServiceModel>()
                .Passing(m=> Assert.Equal(m.Products.Count, productCount)));
    }
}
