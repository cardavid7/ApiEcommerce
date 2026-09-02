using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string? _secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public ApplicationUser? GetUserById(string id)
    {
        return _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return _dbContext.ApplicationUsers.OrderBy(u => u.UserName).ToList();
    }

    public bool IsUniqueUser(string username)
    {
        return !_dbContext.ApplicationUsers.Any(u => u.UserName != null && u.UserName.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
    {
        if (string.IsNullOrEmpty(userLoginDto.UserName))
        {
            return new UserLoginResponseDto()
            {
                IsSuccess = false,
                User = null,
                Token = string.Empty,
                Message = "Username is required"
            };
        }

        var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());
        if (user == null)
        {
            return new UserLoginResponseDto()
            {
                IsSuccess = false,
                User = null,
                Token = string.Empty,
                Message = "Username not found"
            };
        }

        if (string.IsNullOrEmpty(userLoginDto.Password))
        {
            return new UserLoginResponseDto()
            {
                IsSuccess = false,
                User = null,
                Token = string.Empty,
                Message = "Password is required"
            };
        }

        bool IsPasswordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!IsPasswordValid)
        {
            return new UserLoginResponseDto()
            {
                IsSuccess = false,
                User = null,
                Token = string.Empty,
                Message = "Credentials are incorrect"
            };
        }

        //JWT
        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            throw new InvalidOperationException("Secret key is not configured");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var key = Encoding.UTF8.GetBytes(_secretKey);
        var handlerToken = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim("username", user.UserName ?? string.Empty)
        };
        // emitir un claim de rol por cada rol asignado al usuario
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handlerToken.CreateToken(tokenDescriptor);
        var userData = _mapper.Map<UserDataDto>(user);
        userData.Roles = roles.ToList();
        return new UserLoginResponseDto()
        {
            IsSuccess = true,
            Token = handlerToken.WriteToken(token),
            User = userData,
            Message = "User successfully logged in"
        };
    }

    public async Task<UserRegisterResponseDto> Register(CreateUserDto createUserDto)
    {
        if (string.IsNullOrEmpty(createUserDto.UserName))
        {
            return new UserRegisterResponseDto
            {
                IsSuccess = false,
                Message = "UserName is required"
            };
        }

        if (string.IsNullOrEmpty(createUserDto.Password))
        {
            return new UserRegisterResponseDto
            {
                IsSuccess = false,
                Message = "Password is required"
            };
        }

        // UserManager.CreateAsync se encarga de calcular NormalizedUserName y
        // NormalizedEmail, no hace falta asignarlos a mano.
        var user = new ApplicationUser()
        {
            UserName = createUserDto.UserName,
            Email = createUserDto.UserName,
            Name = createUserDto.Name
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new UserRegisterResponseDto
            {
                IsSuccess = false,
                Message = $"Error registering the user: {errors}"
            };
        }

        var userRole = createUserDto.Role ?? "User";
        bool roleExist = await _roleManager.RoleExistsAsync(userRole);
        if (!roleExist)
        {
            var identityRole = new IdentityRole(userRole);
            await _roleManager.CreateAsync(identityRole);
        }
        await _userManager.AddToRoleAsync(user, userRole);

        var createdUser = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower().Trim() == createUserDto.UserName.ToLower().Trim());
        var userData = _mapper.Map<UserDataDto>(createdUser);
        userData.Roles = createdUser != null ? (await _userManager.GetRolesAsync(createdUser)).ToList() : new List<string>();
        return new UserRegisterResponseDto
        {
            IsSuccess = true,
            Message = "User successfully registered",
            User = userData
        };
    }
}
