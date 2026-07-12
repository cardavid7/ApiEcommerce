using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class CreateProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Price is required")]
    public decimal Price { get; set; }
    public string ImgUrl { get; set; } = string.Empty;
    [Required(ErrorMessage = "SKU is required")]
    public string SKU { get; set; } = string.Empty;
    [Required(ErrorMessage = "Stock is required")]
    public int Stock { get; set; }
    [Required(ErrorMessage = "Category ID is required")]
    public int CategoryId { get; set; }
}
