using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.Models;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Users;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(WineCooperativeDbContext data, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.data = data;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateModel user)
        {
            if (user.Password!=user.ConfirmPassword)
            {
                this.ModelState.AddModelError(nameof(user.ConfirmPassword), "The password confirmation is must be the same as the password!");
            }

            if (!this.ModelState.IsValid)
            {
                return View(user);
            }

            var createUser = new User
            {
                UserName = user.Username,
                Email = user.Email,
            };

            var succes = await this.userManager.CreateAsync(createUser, user.Password);

            if(!succes.Succeeded)
            {
                var errors = succes.Errors.Select(e => e.Description);

                foreach (var error in errors)
                {
                    this.ModelState.AddModelError(string.Empty, error);
                }

                return View(user);
            }

            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel user)
        {
            var loggedinUser = await this.userManager.FindByNameAsync(user.Username);

            if(loggedinUser == null)
            {
                this.InvalidCredentials(user);
            }

            var checkPassword = await this.userManager.CheckPasswordAsync(loggedinUser,user.Password);

            if(!checkPassword)
            {
                this.InvalidCredentials(user);
            }

            await this.signInManager.SignInAsync(loggedinUser,false);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult AdditionalUserInfo() => View();

        [HttpPost]
        [Authorize]
        public IActionResult AdditionalUserInfo(AdditionalUserInfoAddingModel userInfo)
        {
            if(!this.ModelState.IsValid)
            {
                return View(userInfo);
            }

            var country = data.Countries
                .Where(c => c.CountryName == userInfo.Address.CountryName)
                .FirstOrDefault();

            if(country == null)
            {
                country = new Country
                {
                    CountryName = userInfo.Address.CountryName
                };

                data.Countries.Add(country);
                data.SaveChanges();
            }

            var town = data.Towns
                .Where(t => t.Name == userInfo.Address.TownName)
                .FirstOrDefault();

            if(town == null)
            {
                town = new Town
                {
                    Name = userInfo.Address.TownName,
                    Country = country
                };
            }

            var address = new Address
            {
                Street = userInfo.Address.Street,
                ZipCode = userInfo.Address.ZipCode,
                Town = town,                 
            };

            var userInformation = new UserAdditionalInformation
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Address = address,
            };

            return RedirectToAction("Index","Home");
        }

        public IActionResult MyProducts()
        {
            var products = data.Products
                .Where(p => p.Manufacturer.UserId == this.User.GetId())
                .Select(p => new UserProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    Price = p.Price,
                    Color = p.Color.Name,
                    Taste = p.Taste.Name,
                    ManufactureYear = p.ManufactureYear,
                    Manufacturer = p.Manufacturer.Name,
                    InStock = p.InStock,
                    WineArea = p.WineArea.Name,
                })
                .ToList();

            return View(products);
        }

        public IActionResult MyServices()
        {
            return View();
        }

        private IActionResult InvalidCredentials(UserLoginModel user)
        {
            const string invalidMessage = "Credentials invalid!";

            this.ModelState.AddModelError(string.Empty, invalidMessage);

            return View(user);
        }
    }
}
