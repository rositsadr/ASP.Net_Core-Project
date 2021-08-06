﻿using AutoMapper;
using System.Linq;
using Web.Data.Models;
using Web.Models;
using Web.Models.Manufacturers;
using Web.Models.Products;
using Web.Models.Services;
using Web.Services.Cart.Models;
using Web.Services.Manufacturers.Models;
using Web.Services.Products.Models;
using Web.Services.Services.Models;

namespace Web.Infrastructures
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProductEditServiceModel, ProductModel>();
            this.CreateMap<Product, ProductServiceModel>();
            this.CreateMap<Product, ProductDetailsServiceModel>()
                .ForMember(pd => pd.Taste, p => p.MapFrom(p => p.Taste.Name))
                .ForMember(pd => pd.Color, p => p.MapFrom(p => p.Color.Name))
                .ForMember(pd=>pd.GrapeVarieties, p=>p.MapFrom(p=>p.GrapeVarieties.Select(gv=>gv.GrapeVariety.Name)));
            this.CreateMap<Product, ProductEditServiceModel>()
                .ForMember(pesm => pesm.UserId, p => p.MapFrom(p => p.Manufacturer.UserId))
                .ForMember(pesm => pesm.GrapeVarieties, p => p.MapFrom(p => p.GrapeVarieties.Select(gv => gv.GrapeVarietyId)));
            this.CreateMap<WineArea, ProductWineAreaServiceModel>();
            this.CreateMap<GrapeVariety, ProductGrapeVarietiesServiceModel>()
                .ForMember(pg => pg.GrapeVarietyId, gv => gv.MapFrom(gv => gv.Id))
                .ForMember(pg => pg.GrapeVarietyName, gv => gv.MapFrom(gv => gv.Name));
            this.CreateMap<ProductColor, ProductColorServiceModel>();
            this.CreateMap<ProductTaste, ProductTasteServiceModel>();

            this.CreateMap<Address, ManufacturerAddressServiceModel>()
                .ForMember(masm => masm.TownName, t => t.MapFrom(t => t.Town.Name));
            this.CreateMap<Manufacturer, ManufacturerServiceModel>();
            this.CreateMap<Manufacturer, ManufacturerNameServiceModel>();
            this.CreateMap<ManufacturerServiceModel, ManufacturerModel>();
            this.CreateMap<ManufacturerAddressServiceModel, ManufacturerAddressViewModel>();

            this.CreateMap<Service, ServiceDetailsIdServiceModel>()
                .ForMember(sd=>sd.UserId, s=>s.MapFrom(s=>s.Manufacturer.UserId));
            this.CreateMap<Service, ServiceDetailsServiceModel>();
            this.CreateMap<ServiceDetailsIdServiceModel, ServiceModel>();

            this.CreateMap<CartItem, CartItemViewServiceModel>();
        }
    }
}
