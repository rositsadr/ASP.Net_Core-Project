using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.Models;
using static Web.WebConstants;

namespace Web.Infrastructures
{
    public static class ApplicationBuilderExtentions
    {
        public static IApplicationBuilder PrepareDatabase(this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices
                .CreateScope();

            var serviceProvider = scopedServices.ServiceProvider;

            var data = serviceProvider
                .GetRequiredService<WineCooperativeDbContext>();

            data.Database.Migrate();

            SeedWineAreas(data);
            SeedProductColor(data);
            SeedProductTaste(data);
            SeedGrapeVarieties(data);
            SeedAdministrator(serviceProvider);
            SeedRole(serviceProvider, MemberRole);

            return app;
        }

        private static void SeedWineAreas(WineCooperativeDbContext data)
        {
            if (!data.WineAreas.Any())
            {

                data.WineAreas.AddRange(new[]
                {
                new WineArea {Name = "Black Sea Coast"},
                new WineArea {Name = "Danube River Plains"},
                new WineArea {Name = "Struma Valley"},
                new WineArea {Name = "Rose Valley"},
                new WineArea {Name = "Thracian Valley"},
            });

                data.SaveChanges();
            }
        }

        private static void SeedProductColor(WineCooperativeDbContext data)
        {
            if (!data.ProductColors.Any())
            {

                data.ProductColors.AddRange(new[]
                {
                new ProductColor {Name = "Red"},
                new ProductColor {Name = "Withe"},
                new ProductColor {Name = "Rose"},
            });

                data.SaveChanges();
            }
        }

        private static void SeedProductTaste(WineCooperativeDbContext data)
        {
            if (!data.ProductTastes.Any())
            {

                data.ProductTastes.AddRange(new[]
                {
                new ProductTaste {Name = "Sweet"},
                new ProductTaste {Name = "Dry"},
                new ProductTaste {Name = "Semy-dry"},
            });

                data.SaveChanges();
            }
        }

        private static void SeedGrapeVarieties(WineCooperativeDbContext data)
        {
            if (!data.GrapeVarieties.Any())
            {

                data.GrapeVarieties.AddRange(new[]
                {
                new GrapeVariety {Name = "Merlot"},
                new GrapeVariety {Name = "Sauvignon blanc"},
                new GrapeVariety {Name = "Pinot gris"},
                new GrapeVariety {Name = "Semillon"},
                new GrapeVariety {Name = "Viognier"},
                new GrapeVariety {Name = "Muscat"},
                new GrapeVariety {Name = "Riesling"},
                new GrapeVariety {Name = "Marsanne"},
                new GrapeVariety {Name = "Cabernet Franc"},
                new GrapeVariety {Name = "Cabernet Sauvignon"},
                new GrapeVariety {Name = "Malbec"},
                new GrapeVariety {Name = "Grenache"},
                new GrapeVariety {Name = "Tempranillo"},
                new GrapeVariety {Name = "Primitivo"},
                new GrapeVariety {Name = "Syrah"},
                new GrapeVariety {Name = "Zinfandel"},
                new GrapeVariety {Name = "Sangiovese"},
                new GrapeVariety {Name = "Pinot Noir"},
                new GrapeVariety {Name = "Rubin"},
                new GrapeVariety {Name = "Ruen"},
                new GrapeVariety {Name = "Mavrud"},
                new GrapeVariety {Name = "Gumza"},
                new GrapeVariety {Name = "Melnishka Loza"},
                new GrapeVariety {Name = "Melnik 55"},
                new GrapeVariety {Name = "Vrachanski Misket"},
                new GrapeVariety {Name = "Singurlarski Misket"},
                new GrapeVariety {Name = "Varnenski Misket"},
                new GrapeVariety {Name = "Traminer"},
                new GrapeVariety {Name = "Chardonay"},
            });

                data.SaveChanges();
            }
        }

        private static void SeedAdministrator(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var created = SeedRole(serviceProvider, AdministratorRole);

            Task.Run(async () =>
            {
                if (created)
                {
                    const string adminUserName = "Administrator";
                    const string adminEmail = "administrator@winec.bg";
                    const string adminPassword = "admin132";

                    var user = new User
                    {
                        UserName = adminUserName,
                        Email = adminEmail
                    };

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, AdministratorRole);
                }
            })
                .GetAwaiter()
                .GetResult();
        }

        private static bool SeedRole(IServiceProvider serviceProvider,string roleName)
        {
            var created = false;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            Task.Run(async () =>
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var newRole = new IdentityRole { Name = roleName };

                    await roleManager.CreateAsync(newRole);

                    created = true;
                }
            })
                .GetAwaiter()
                .GetResult();

            return created;
        }
    }
}
