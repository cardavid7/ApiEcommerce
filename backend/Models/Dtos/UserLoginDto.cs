using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models.Dtos;

public class UserLoginDto
{
    [Required(ErrorMessage = "Username is required")]
    [EmailAddress(ErrorMessage = "Username must be a valid email address")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
