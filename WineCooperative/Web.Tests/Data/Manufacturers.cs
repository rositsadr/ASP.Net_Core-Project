using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Models;
using static Web.WebConstants;

namespace Web.Tests.Data
{
    public static class Manufacturers
    {

        public static IEnumerable<Manufacturer> FiveManufacturers =>
            Enumerable.Range(0, 5).Select(i => new Manufacturer
            {                
                IsFunctional = true
            });

        public static string ManufacturerId =>
            new Guid().ToString();

        public static string ProductId =>
           new Guid().ToString();
    }
}
