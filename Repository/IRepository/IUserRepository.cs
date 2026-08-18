using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    public ICollection<ApplicationUser> GetUsers();
    public ApplicationUser? GetUserById(string id);
    public bool IsUniqueUser(string username);
    public Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
    public Task<UserDataDto> Register(CreateUserDto createUserDto);
}
