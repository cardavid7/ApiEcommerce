using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    public ICollection<User> GetUsers();
    public User? GetUserById(int id);
    public bool IsUniqueUser(string username);

    public Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);

    public Task<User> Register(CreateUserDto createUserDto);
}
