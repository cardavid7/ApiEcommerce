using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster; // Antes: using AutoMapper; (migrado a Mapster, se usa el metodo de extension Adapt())
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        // Con Mapster se usa el metodo de extension Adapt(); ya no se inyecta un mapper.
        // Antes (AutoMapper): private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        // Antes: UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            // Antes: _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userRepository.GetUsers();
            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = user.Adapt<UserDto>(); // Antes: _mapper.Map<UserDto>(user);
                userDto.Roles = await _userManager.GetRolesAsync(user);
                usersDto.Add(userDto);
            }
            return Ok(usersDto);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var userDto = user.Adapt<UserDto>(); // Antes: _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost("Register", Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(createUserDto.UserName))
            {
                return BadRequest("UserName is required");
            }
            if (!_userRepository.IsUniqueUser(createUserDto.UserName))
            {
                return BadRequest("UserName already exists");
            }

            // Este endpoint es publico (AllowAnonymous): se ignora el rol que mande el
            // cliente para evitar que cualquiera se autoasigne "Admin" al registrarse.
            createUserDto.Role = "User";

            var result = await _userRepository.Register(createUserDto);
            if (!result.IsSuccess || result.User == null)
            {
                return BadRequest(result.Message);
            }
            return CreatedAtRoute("GetUserById", new { id = result.User.Id }, result.User);
        }

        [AllowAnonymous]
        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userRepository.Login(userLoginDto);
            if (!response.IsSuccess)
            {
                return Unauthorized(response.Message);
            }
            return Ok(response);
        }
    }
}
