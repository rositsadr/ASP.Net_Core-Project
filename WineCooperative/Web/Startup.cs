using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Data;
using Web.Data.Models;
using Web.Infrastructures;
using Web.Services.Addresses;
using Web.Services.Cart;
using Web.Services.Manufacturers;
using Web.Services.Orders;
using Web.Services.Products;
using Web.Services.Services;
using Web.Services.Users;
using Web.Services.WineAreas;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => 
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WineCooperativeDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WineCooperativeDbContext>();

            services.AddAutoMapper(typeof(Startup));

            services.AddMemoryCache();

            services.AddSession(options=>
            {
                options.Cookie.HttpOnly = true;
            });

            services.AddControllersWithViews(options=>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            }
            );

            services.AddHttpContextAccessor();

            services.AddTransient<IProductService, ProductService>(); 
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IServiceService, ServiceService>();
            services.AddTransient<IManufacturerService, ManufacturerService>();
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<ICartService, CartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IWineAreasService, WineAreaService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.PrepareDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Areas",
                    pattern: "/{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
