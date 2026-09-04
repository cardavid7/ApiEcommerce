using System;

namespace ApiEcommerce.Models.Dtos;

public class UserRegisterResponseDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public UserDataDto? User { get; set; }
}
