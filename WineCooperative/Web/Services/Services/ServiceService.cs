﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Models;
using Web.Models.Services;
using Web.Models.Services.Enums;
using Web.Services.Services.Models;

namespace Web.Services.Services
{
    public class ServiceService:IServiceService
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
                DateCreated = DateTime.UtcNow.ToString("r"),
                Available = available
            };

            data.Services.Add(serviceToAdd);
            data.SaveChanges();
        }

        public ServiceSearchPageServiceModel All(int servicesPerRage, int currantPage, string searchTerm, ServiceSort sorting)
        {
            var servicesQuery = data.Services
                .AsQueryable();

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

        public bool ServiceExists(string userId, string name) => data.Services
            .Any(s => s.Manufacturer.UserId == userId && s.Name == name);

        public IEnumerable<ServiceDetailsServiceModel> ServicesByUser(string userId) => this.data.Services
            .Where(s => s.Manufacturer.UserId == userId)
            .ProjectTo<ServiceDetailsServiceModel>(config)
            .ToList();
    }
}
