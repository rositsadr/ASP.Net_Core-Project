using MyTested.AspNetCore.Mvc;
using Web.Controllers;
using Web.ViewModels;
using Xunit;

namespace Web.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexShouldReturnView()
            => MyMvc
            .Pipeline()
            .ShouldMap("/")
            .To<HomeController>(c => c.Index())
            .Which()
            .ShouldReturn()
            .View();

        [Fact]
        public void ErrorShouldReturnViewWithModel()
            => MyMvc
            .Pipeline()
            .ShouldMap("Home/Error")
            .To<HomeController>(c => c.Error())
            .Which()
            .ShouldReturn()
            .View(view => view
                    .WithModelOfType<ErrorViewModel>());
    }
}
