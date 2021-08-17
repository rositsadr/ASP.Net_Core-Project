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
                    new WineArea 
                    {
                    Name = "Black Sea Coast",
                    Description = "The Black Sea region is where 30% of all vines are located. The region is characterized by long and mild autumns that are a favourable condition for the accumulation of sugars to make fine white wine (53% of all white wine varietals are concentrated in the region). Wine styles include Dimyat, Riesling, Muscat Ottonel, Ugni blanc, Sauvignon blanc, Traminer, and Gewürztraminer. In the US appellation also approved as \"Black Sea Coastal\" region."
                    },
                    new WineArea 
                    {
                        Name = "Danube River Plains",
                        Description="The Danubian Plain or North Bulgarian region encompasses the south banks of the Danube and the central and western parts of the Danubian Plain. The climate of the area is temperate continental, has a hot summer and many sunny days a year. Typical styles are Muscat Ottonel, Cabernet Sauvignon, Merlot, Chardonnay, Aligoté, Pamid and the local Gamza. In the US appellation also approved as \"Danube River Plains\" region."
                    },
                    new WineArea 
                    {
                        Name = "Struma Valley",
                        Description="The region includes the southwestern parts of Bulgaria, the valley of the river Struma in the historical region of Macedonia. The area is small in size, but is climatically very distinct and characteristic, owing to the strong Mediterranean influence from the south. The local style Shiroka melnishka loza (taking its name from Melnik), as well as Cabernet Sauvignon and Merlot are cultivated."
                    },
                    new WineArea 
                    {
                        Name = "Rose Valley",
                        Description="The Rose Valley region is located south of the Balkan Mountains. It is divided into an eastern and western subregion, with styles such as Muscatel, Riesling, Rkatsiteli, Cabernet Sauvignon and Merlot dominating. The region mostly produces dry and off-dry white wine and less red wine. The region includes the Sungurlare Valley, famous for its wine from the Red Misket grape variety. In the US appellation also approved as \"Valley of the Roses\" region."
                    },
                    new WineArea 
                    {
                        Name = "Thracian Valley",
                        Description = "The temperate continental climate in the area and the favourable distribution of precipitation are good premises for the developed red wine growing in the lowlands of Upper Thrace. The region includes the central part of the lowland, as well as parts of the Sakar mountain. Mavrud, a famous local wine, as well as Merlot, Cabernet Sauvignon, Muscatel and Pamid are grown.The Balkan Mountains serve to block the cold winds blowing from the plains of Russia, and the region to the south of the Balkans, the valley drained by the Maritsa River, has a Mediterranean climate, with mild, rainy winters and warm, dry summers. In the US appellation also approved as \"Thracian Valley\" region."
                    },
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
