using AutoMapper;
using System.Linq;
using Web.Data.Models;
using Web.ViewModels.Manufacturers;
using Web.ViewModels.Products;
using Web.ViewModels.Services;
using Web.Services.Addresses;
using Web.Services.Cart.Models;
using Web.Services.Manufacturers.Models;
using Web.Services.Products.Models;
using Web.Services.Services.Models;
using Web.Services.Users.Models;
using Web.Services.WineAreas.Models;

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

            this.CreateMap<Service, ServiceDetailsIdServiceModel>()
                .ForMember(sd=>sd.UserId, s=>s.MapFrom(s=>s.Manufacturer.UserId));
            this.CreateMap<Service, ServiceDetailsServiceModel>();
            this.CreateMap<ServiceDetailsIdServiceModel, ServiceModel>();

            this.CreateMap<CartItem, CartItemViewServiceModel>();

            this.CreateMap<User, UserInfoServiceModel>()
                .ForMember(ui => ui.FirstName, u => u.MapFrom(u => u.UserData.FirstName))
                .ForMember(ui => ui.LastName, u => u.MapFrom(u => u.UserData.LastName));
            this.CreateMap<Address, AddressEditServiceModel>()
                .ForMember(ae => ae.TownName, a => a.MapFrom(a => a.Town.Name))
                .ForMember(ae => ae.CountryName, a => a.MapFrom(a => a.Town.Country.CountryName));
            this.CreateMap<User, UserEditInfoServiceModel>()
                .ForMember(ue => ue.UserId, u => u.MapFrom(u => u.Id));
            this.CreateMap<UserAdditionalInformation, UserEditInfoServiceModel>()
                .ForMember(ue => ue.UserId, u => u.MapFrom(u => u.UserId))
                .ForMember(ue => ue.PhoneNumber, u => u.MapFrom(u => u.User.PhoneNumber));

            this.CreateMap<Product, WineAreasProductsServiceModel>();
            this.CreateMap<WineArea, WineAreaDisplayServiceModel>();
            this.CreateMap<WineArea, WineAreaServiceModel>();
        }
    }
}
