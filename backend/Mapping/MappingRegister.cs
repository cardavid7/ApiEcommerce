using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using Mapster;

namespace ApiEcommerce.Mapping;

// Configuracion de Mapster (reemplaza a los Profile de AutoMapper).
// TypeAdapterConfig.Scan() detecta automaticamente las clases que implementan IRegister.
// Mapster mapea por convencion (mismo nombre de propiedad), asi que aqui solo se
// declaran los mapeos que necesitan reglas personalizadas; el resto funciona sin config.
//
// Equivalencias con los antiguos Profile:
//   CategoryProfile  -> Category <-> CategoryDto / CreateCategoryDto  (por convencion)
//   UserProfile      -> ApplicationUser <-> UserDataDto / UserDto     (por convencion)
//   ProductProfile   -> Product <-> CreateProductDto / UpdateProductDto (por convencion)
//                       Product -> ProductDto necesita mapear CategoryName (ver abajo)
public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Antes (AutoMapper - ProductProfile):
        // CreateMap<Product, ProductDto>()
        //     .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        //     .ReverseMap();
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.CategoryName, src => src.Category!.Name);
    }
}
