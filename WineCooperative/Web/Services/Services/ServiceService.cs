using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Web.Data;
using Web.Models;
using Web.Models.Services;
using Web.Models.Services.Enums;
using Web.Services.Services.Models;

namespace Web.Services.Services
{
    public class ServiceService : IServiceService
    {
        private readonly WineCooperativeDbContext data;
        private readonly IConfigurationProvider config;

        public ServiceService(WineCooperativeDbContext data, IMapper mapper)
        {
            this.data = data;
            this.config = mapper.ConfigurationProvider;
        }

        public void Create(string name, decimal price, string imageUrl, string description, string manufacturerId, bool available)
        {
            var serviceToAdd = new Service
            {
                Name = name,
                Price = price,
                ImageUrl = imageUrl,
                Description = description,
                ManufacturerId = manufacturerId,
                DateCreated = DateTime.UtcNow.ToString("r",CultureInfo.CreateSpecificCulture("bg-BG")),
                Available = available
            };

            data.Services.Add(serviceToAdd);
            data.SaveChanges();
        }

        public ServiceSearchPageServiceModel AllAvailable(int servicesPerRage, int currantPage, string searchTerm, ServiceSort sorting)
        {
            var servicesQuery = data.Services
                .Where(s=>s.Available)
                .AsQueryable();

            return this.SearchPage(servicesQuery, servicesPerRage, currantPage, searchTerm, sorting);
        }

        public ServiceSearchPageServiceModel All(int servicesPerRage, int currantPage, string searchTerm, ServiceSort sorting)
        {
            var servicesQuery = data.Services
                .AsQueryable();

            return this.SearchPage(servicesQuery, servicesPerRage, currantPage, searchTerm, sorting);
        }

        public ServiceDetailsIdServiceModel Edit(string serviceId) => this.GetService(serviceId);

        public bool ApplyChanges(string serviceId, string name, string description, string imageUrl, decimal price, bool available, string manufacturerId)
        {
            var service = data.Services
                .Where(s => s.Id == serviceId)
                .FirstOrDefault();

            if (service == null)
            {
                return false;
            }

            bool dateChange = false;

            if (service.Name != name)
            {
                service.Name = name;
                dateChange = true;
            }

            if(service.Description != description)
            {
                service.Description = description;
                dateChange = true;
            }

            if (service.ImageUrl != imageUrl)
            {
                service.ImageUrl = imageUrl;
                dateChange = true;
            }

            if (service.Price!=price)
            {
                service.Price = price;
                dateChange = true;
            }

            if(service.Available != available)
            {
                service.Available = available;
                dateChange = true;
            }

            if (service.ManufacturerId != manufacturerId)
            {
                service.ManufacturerId = manufacturerId;
                dateChange = true;
            }

            if(dateChange)
            {
                service.DateCreated = DateTime.UtcNow.ToString("r", CultureInfo.CreateSpecificCulture("bg-BG"));
            }

            data.SaveChanges();

            return true;
        }

        public ServiceDetailsIdServiceModel Details(string serviceId) => this.GetService(serviceId);

        public bool Delete(string serviceId)
        {
            var service = data.Services
               .Find(serviceId);

            if (service != null)
            {
                data.Services.Remove(service);
                data.SaveChanges();

                return true;
            }

            return false;
        }

        public bool ServiceExists(string manufacturerId, string name) => data.Services
            .Any(s => s.ManufacturerId == manufacturerId && s.Name == name && s.Available);

        public IEnumerable<ServiceDetailsServiceModel> ServicesByUser(string userId) => this.data.Services
            .Where(s => s.Manufacturer.UserId == userId)
            .ProjectTo<ServiceDetailsServiceModel>(config)
            .ToList();

        public bool IsUsersService(string userId, string serviceId) => data.Services
             .Any(s => s.Id == serviceId && s.Manufacturer.UserId == userId);

        private ServiceDetailsIdServiceModel GetService(string serviceId) => data.Services
            .Where(s=>s.Id == serviceId)
            .ProjectTo<ServiceDetailsIdServiceModel>(config)
            .FirstOrDefault();

        private ServiceSearchPageServiceModel SearchPage(IQueryable<Service> servicesQuery, int servicesPerRage, int currantPage, string searchTerm, ServiceSort sorting)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                servicesQuery = servicesQuery
                    .Where(p => (p.Name.ToLower() + " " + p.Description.ToLower()).Contains(searchTerm));
            }

            servicesQuery = sorting switch
            {
                ServiceSort.ByDate => servicesQuery.OrderByDescending(p => p.DateCreated),
                ServiceSort.ByManufacturer => servicesQuery.OrderBy(p => p.Manufacturer.Name),
                ServiceSort.ByName or _ => servicesQuery.OrderBy(p => p.Name)
            };

            var totalServices = servicesQuery.Count();

            var services = servicesQuery
                .Skip((currantPage - 1) * servicesPerRage)
                 .Take(servicesPerRage)
                 .ProjectTo<ServiceDetailsIdServiceModel>(config)
                 .ToList();

            return new ServiceSearchPageServiceModel
            {
                TotalServices = totalServices,
                CurrantPage = currantPage,
                ServicesPerPage = servicesPerRage,
                Services = services,
            };
        }
    }
}
