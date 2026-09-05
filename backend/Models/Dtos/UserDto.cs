using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class UserDto
{
    [Required(ErrorMessage = "Id is required")]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Username is required")]
    public string? UserName { get; set; }

    public ICollection<string> Roles { get; set; } = new List<string>();
}
