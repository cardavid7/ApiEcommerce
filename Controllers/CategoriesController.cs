using ApiEcommerce.Constants;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetAllCategories();
            //var categoriesDto = _mapper.Map<ICollection<CategoryDto>>(categories);
            
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            
            return Ok(categoriesDto);
        }

        [AllowAnonymous]
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
            var categoryDto = _mapper.Map<CategoryDto>(category);
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

            var category = _mapper.Map<Category>(categoryCreateDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {category.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoryById", new { id = category.Id }, _mapper.Map<CategoryDto>(category));
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

            if (_categoryRepository.CategoryExists(categoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);
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
