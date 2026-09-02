using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class UserProfile: Profile
{
    public UserProfile()
    {
        // The legacy `User` entity was replaced by `ApplicationUser` (ASP.NET Core Identity)
        // and its table was dropped in the `RemoveLegacyUsersTable` migration. These maps are
        // no longer needed because nothing in the app resolves or persists a `User` instance.
        // CreateMap<User, UserDto>().ReverseMap();
        // CreateMap<User, CreateUserDto>().ReverseMap();
        // CreateMap<User, UserLoginDto>().ReverseMap();
        // CreateMap<User, UserLoginResponseDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDataDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}
