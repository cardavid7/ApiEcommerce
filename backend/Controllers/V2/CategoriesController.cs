using ApiEcommerce.Constants;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster; // Antes: using AutoMapper; (migrado a Mapster, se usa el metodo de extension Adapt())
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.V2
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    //[EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        // Con Mapster se usa el metodo de extension Adapt(); ya no se inyecta un mapper.
        // Antes (AutoMapper): private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository)
        // Antes: CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            // Antes: _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        public IActionResult GetCategoriesOrderById()
        {
            var categories = _categoryRepository.GetAllCategories().OrderBy(cat => cat.Id);
            //var categoriesDto = categories.Adapt<ICollection<CategoryDto>>();

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(category.Adapt<CategoryDto>()); // Antes: _mapper.Map<CategoryDto>(category)
            }
            
            return Ok(categoriesDto);
        }

        [AllowAnonymous]
        // [ResponseCache(Duration = 10)]
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            var categoryDto = category.Adapt<CategoryDto>(); // Antes: _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto categoryCreateDto)
        {
            if(categoryCreateDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(categoryCreateDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            var category = categoryCreateDto.Adapt<Category>(); // Antes: _mapper.Map<Category>(categoryCreateDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {category.Name}");
                return StatusCode(500, ModelState);
            }
            // Antes: _mapper.Map<CategoryDto>(category)
            return CreatedAtRoute("GetCategoryById", new { id = category.Id }, category.Adapt<CategoryDto>());
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, [FromBody] CreateCategoryDto categoryDto)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with ID {id} not found.");
            }

            if (categoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(categoryDto.Name, id))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            var category = categoryDto.Adapt<Category>(); // Antes: _mapper.Map<Category>(categoryDto);
            category.Id = id;
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the record {category.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok(categoryDto);
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting the record {category.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
