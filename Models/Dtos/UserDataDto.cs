using System;

namespace ApiEcommerce.Models.Dtos;

public class UserDataDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public ICollection<string> Roles { get; set; } = new List<string>();
}
