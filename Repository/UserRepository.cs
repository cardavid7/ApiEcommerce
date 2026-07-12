using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private string? secretKey;
    public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }

    public User? GetUserById(int id)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return _dbContext.Users.OrderBy(u => u.UserName).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        return !_dbContext.Users.Any(u => u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.UserName))
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = string.Empty,
                Message = "Username is required"
            };
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync<User>(u => u.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = string.Empty,
                Message = "Username not found"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            return new UserLoginResponseDto()
            {
                User = null,
                Token = string.Empty,
                Message = "Credentials are incorrect"
            };
        }

        //JWT
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Secret key is not configured");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var handlerToken = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto()
            {
                UserName = user.UserName,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password ?? ""
            },
            Message = "User successfully logged in"
        };
    }

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = new User()
        {
            Name = createUserDto.Name,
            UserName = createUserDto.UserName,
            Password = encryptedPassword,
            Role = createUserDto.Role
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}
