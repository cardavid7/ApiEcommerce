// ============================================================================
// ARCHIVO DE REFERENCIA - AutoMapper (YA NO SE USA)
// ----------------------------------------------------------------------------
// La aplicacion migro de AutoMapper a Mapster.
// La configuracion equivalente ahora vive en Mapping/MappingRegister.cs.
// Este archivo se conserva unicamente como referencia historica de como se
// definian los mapeos con AutoMapper.
// ============================================================================
//
// using System;
// using ApiEcommerce.Models;
// using ApiEcommerce.Models.Dtos;
// using AutoMapper;
//
// namespace ApiEcommerce.Mapping;
//
// public class ProductProfile: Profile
// {
//     public ProductProfile()
//     {
//         CreateMap<Product, ProductDto>()
//             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
//             .ReverseMap();
//         CreateMap<Product, CreateProductDto>().ReverseMap();
//         CreateMap<Product, UpdateProductDto>().ReverseMap();
//     }
// }
