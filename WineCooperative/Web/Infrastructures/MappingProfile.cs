using AutoMapper;
using System.Linq;
using Web.Models;
using Web.Models.Products;
using Web.Models.Services;
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
            this.CreateMap<Product, ProductDetailsServiceModel>();
            this.CreateMap<Product, ProductEditServiceModel>()
                .ForMember(pesm => pesm.UserId, p => p.MapFrom(x => x.Manufacturer.UserId))
                .ForMember(pesm => pesm.GrapeVarieties, p => p.MapFrom(x => x.GrapeVarieties.Select(gv => gv.GrapeVarietyId)));
            this.CreateMap<WineArea, ProductWineAreaServiceModel>();
            this.CreateMap<GrapeVariety, ProductGrapeVarietiesServiceModel>()
                .ForMember(pg => pg.GrapeVarietyId, gv => gv.MapFrom(g => g.Id))
                .ForMember(pg => pg.GrapeVarietyName, gv => gv.MapFrom(g => g.Name));
            this.CreateMap<ProductColor, ProductColorServiceModel>();
            this.CreateMap<ProductTaste, ProductTasteServiceModel>();

            this.CreateMap<Address, ManufacturerAddressServiceModel>()
                .ForMember(masm => masm.TownName, a => a.MapFrom(t => t.Town.Name));
            this.CreateMap<Manufacturer, ManufacturerServiceModel>();
            this.CreateMap<Manufacturer, ManufacturerNameServiceModel>();

            this.CreateMap<Service, ServiceDetailsIdServiceModel>();
            this.CreateMap<Service, ServiceDetailsServiceModel>();
            this.CreateMap<ServiceDetailsIdServiceModel, ServiceModel>();
        }
    }
}
