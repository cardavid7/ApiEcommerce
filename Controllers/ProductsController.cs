using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    //Authorize(Roles = "Admin")], these endpoints are accessible to both Admin and User roles, so we will authorize them at the method level.
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetAllProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("Paginated", Name = "GetProductsPaginated")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsPaginated([FromQuery] PaginationDto paginationDto)
        {
            var totalProducts = _productRepository.GetTotalProducts();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)paginationDto.PageSize);

            if (paginationDto.PageNumber > totalPages && totalPages > 0)
            {
                return BadRequest($"Page number {paginationDto.PageNumber} exceeds the total number of pages ({totalPages}).");
            }

            var products = _productRepository.GetAllProductsInPages(paginationDto.PageNumber, paginationDto.PageSize);
            var productsDto = _mapper.Map<List<ProductDto>>(products);

            var response = new PaginatedResponseDto<ProductDto>
            {
                TotalCount = totalProducts,
                PageSize = paginationDto.PageSize,
                CurrentPage = paginationDto.PageNumber,
                TotalPages = totalPages,
                Items = productsDto,
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductById(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"Category with ID {createProductDto.CategoryId} does not exist.");
                return BadRequest(ModelState);
            }

            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", $"Product with name {createProductDto.Name} already exists.");
                return BadRequest($"Product with name {createProductDto.Name} already exists.");
            }
            var product = _mapper.Map<Product>(createProductDto);

            //add product image
            if (createProductDto.Image != null && createProductDto.Image.Length > 0)
            {
                UploadProductImage(product, createProductDto.Image);
            }
            else
            {
                product.ImgUrl = "https://placehold.co/300x300";
            }

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the product {product.Name}.");
                return StatusCode(500, ModelState);
            }

            var createdProduct = _productRepository.GetProductById(product.Id);
            return CreatedAtRoute("GetProductById", new { id = product.Id }, _mapper.Map<ProductDto>(createdProduct));
        }

        [AllowAnonymous]
        [HttpGet("SearchByCategory/{categoryId:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductsForCategory(int categoryId)
        {
            var products = _productRepository.GetProductsForCategory(categoryId);
            if (products.Count == 0)
            {
                return NotFound($"No products found for category with ID {categoryId}.");
            }
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [AllowAnonymous]
        [HttpGet("SearchByNameOrDescription/{searchItem}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProducts(string searchItem)
        {
            var products = _productRepository.SearchProducts(searchItem);
            if (products.Count == 0)
            {
                return NotFound($"Products with name or description {searchItem} not found.");
            }   
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPatch("BuyProduct/{name}/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                return BadRequest("Invalid product name or quantity.");
            }

            if (!_productRepository.ProductExists(name))
            {
                return NotFound($"Product with name {name} not found.");
            }

            if (!_productRepository.BuyProduct(name, quantity))
            {
                return NotFound($"Product with name {name} not found or insufficient stock.");
            }
            return Ok($"Product {name} purchased successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateProduct(int id, [FromForm] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null || id <= 0)
            {
                return BadRequest(ModelState);
            }

            if (!_productRepository.ProductExists(id))
            {
                return NotFound($"Product with ID {id} not found.");
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"Category with ID {updateProductDto.CategoryId} does not exist.");
                return BadRequest(ModelState);
            }

            var existingProduct = _productRepository.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var product = _mapper.Map<Product>(updateProductDto);
            product.Id = id;

            //add product image
            if (updateProductDto.Image != null && updateProductDto.Image.Length > 0)
            {
                UploadProductImage(product, updateProductDto.Image);

                // Delete the old image from the local file system if it exists
                DeleteProductImage(existingProduct.ImgUrlLocal);
            }
            else
            {
                // dont change the ImgUrl and ImgUrlLocal properties if no new image is provided
                product.ImgUrl = existingProduct.ImgUrl;
                product.ImgUrlLocal = existingProduct.ImgUrlLocal;
            }

            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the product {product.Name}.");
                return StatusCode(500, ModelState);
            }

            var updatedProduct = _productRepository.GetProductById(id);
            return Ok(_mapper.Map<ProductDto>(updatedProduct));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ModelState);
            }

            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting the product {product.Name}.");
                return StatusCode(500, ModelState);
            }
            return Ok($"Product {product.Name} deleted successfully.");
        }

        // Save the received image in wwwroot/ProductsImages and assign
        // ImgUrl (public URL) and ImgUrlLocal (physical path) to the product
        private void UploadProductImage(Product product, IFormFile image)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductsImages");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            string filePath = Path.Combine(imagesFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            product.ImgUrl = $"{baseUrl}/ProductsImages/{fileName}";
            product.ImgUrlLocal = filePath;
        }

        // Delete the product image from the local file system if it exists.
        private static void DeleteProductImage(string? imgUrlLocal)
        {
            if (!string.IsNullOrEmpty(imgUrlLocal) && System.IO.File.Exists(imgUrlLocal))
            {
                System.IO.File.Delete(imgUrlLocal);
            }
        }
    }
}
