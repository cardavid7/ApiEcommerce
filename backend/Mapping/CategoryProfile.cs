// ============================================================================
// ARCHIVO DE REFERENCIA - AutoMapper (YA NO SE USA)
// ----------------------------------------------------------------------------
// La aplicacion migro de AutoMapper a Mapster.
// Estos mapeos ahora los resuelve Mapster por convencion (mismo nombre de
// propiedad); ver Mapping/MappingRegister.cs para los casos personalizados.
// Este archivo se conserva unicamente como referencia historica.
// ============================================================================
//
// using AutoMapper;
// using ApiEcommerce.Models;
// using ApiEcommerce.Models.Dtos;
//
// namespace ApiEcommerce.Mapping;
//
// public class CategoryProfile: Profile
// {
//     public CategoryProfile()
//     {
//         CreateMap<Category, CategoryDto>().ReverseMap();
//         CreateMap<Category, CreateCategoryDto>().ReverseMap();
//     }
// }
