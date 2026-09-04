using System;
using System.ComponentModel.DataAnnotations;

namespace ApiEcommerce.Models;

// Legacy model. It is no longer required: authentication and user storage now go through
// `ApplicationUser` (ASP.NET Core Identity), and the "Users" table was dropped in the
// `RemoveLegacyUsersTable` migration. This class is not mapped by `ApplicationDbContext`
// and is kept only for historical reference; it can be safely deleted.
public class User
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string? Password { get; set; }

    public string? Role { get; set; }
}
