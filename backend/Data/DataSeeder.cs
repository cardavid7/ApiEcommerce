using System;
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext appContext, string publicBaseUrl)
    {
        SeedRoles(appContext);
        SeedCategories(appContext);
        SeedUsers(appContext);
        SeedUserRoles(appContext);
        // Los productos se siembran al final porque necesitan las categorías ya
        // persistidas (con su Id real generado por la base de datos).
        SeedProducts(appContext, publicBaseUrl);
    }

    private static void SeedRoles(ApplicationDbContext appContext)
    {
        if (appContext.Roles.Any())
        {
            return;
        }

        appContext.Roles.AddRange(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
        );
        appContext.SaveChanges();
    }

    private static void SeedCategories(ApplicationDbContext appContext)
    {
        if (appContext.Categories.Any())
        {
            return;
        }

        appContext.Categories.AddRange(
            new Category { Name = "Ropa y accesorios", CreationDate = DateTime.UtcNow },
            new Category { Name = "Electrónicos", CreationDate = DateTime.UtcNow },
            new Category { Name = "Deportes", CreationDate = DateTime.UtcNow },
            new Category { Name = "Hogar", CreationDate = DateTime.UtcNow },
            new Category { Name = "Libros", CreationDate = DateTime.UtcNow }
        );
        appContext.SaveChanges();
    }

    private static void SeedUsers(ApplicationDbContext appContext)
    {
        var hasher = new PasswordHasher<ApplicationUser>();

        // Siembra idempotente por usuario: agrega solo los que falten, así se pueden
        // añadir usuarios nuevos aunque la BD ya tenga usuarios sembrados.
        AddUserIfMissing(appContext, hasher, "admin-001", "admin@email.com", "Administrador", "Admin123!");
        AddUserIfMissing(appContext, hasher, "user-001", "user@email.com", "Usuario Regular", "User123!");
        AddUserIfMissing(appContext, hasher, "david-001", "david@email.com", "David", "David123!");

        appContext.SaveChanges();
    }

    private static void AddUserIfMissing(ApplicationDbContext appContext, PasswordHasher<ApplicationUser> hasher,
        string id, string email, string name, string password)
    {
        if (appContext.ApplicationUsers.Any(u => u.Id == id))
        {
            return;
        }

        var user = new ApplicationUser
        {
            Id = id,
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            Name = name
        };
        user.PasswordHash = hasher.HashPassword(user, password);
        appContext.ApplicationUsers.Add(user);
    }

    private static void SeedUserRoles(ApplicationDbContext appContext)
    {
        // "1" = Admin, "2" = User (ver SeedRoles). david-001 tiene ambos roles.
        AddUserRoleIfMissing(appContext, "admin-001", "1");
        AddUserRoleIfMissing(appContext, "user-001", "2");
        AddUserRoleIfMissing(appContext, "david-001", "1");
        AddUserRoleIfMissing(appContext, "david-001", "2");

        appContext.SaveChanges();
    }

    private static void AddUserRoleIfMissing(ApplicationDbContext appContext, string userId, string roleId)
    {
        if (appContext.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == roleId))
        {
            return;
        }

        appContext.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = roleId });
    }

    private static void SeedProducts(ApplicationDbContext appContext, string publicBaseUrl)
    {
        if (appContext.Products.Any())
        {
            return;
        }

        // Se referencia cada categoría por nombre (no por Id fijo) para no depender
        // de que la BD haya generado los Id 1..5 en ese orden.
        var categoriesByName = appContext.Categories.ToDictionary(c => c.Name);

        // Las imagenes viven en wwwroot/ProductsImages/seed (committeadas al repo,
        // a diferencia de las subidas por usuarios), servidas via UseStaticFiles().
        var products = new (string Name, string Description, decimal Price, string Sku, int Stock, string Category, string ImgFileName)[]
        {
            ("Camiseta Básica", "Camiseta de algodón 100%", 25.99m, "PROD-001-CAM-M", 50, "Ropa y accesorios", "camiseta-basica.jpg"),
            ("Smartphone Galaxy", "Teléfono inteligente con 128GB", 599.99m, "PROD-002-PHO-BLK", 25, "Electrónicos", "smartphone-galaxy.jpg"),
            ("Pelota de Fútbol", "Pelota oficial FIFA", 45.00m, "PROD-003-BAL-WHT", 30, "Deportes", "pelota-futbol.jpg"),
            ("Lámpara de Mesa", "Lámpara LED regulable", 89.99m, "PROD-004-LAM-WHT", 15, "Hogar", "lampara-mesa.jpg"),
            ("El Quijote", "Novela clásica de Cervantes", 19.99m, "PROD-005-LIB-ESP", 100, "Libros", "el-quijote.jpg"),
            ("Jeans Clásicos", "Pantalones vaqueros azules", 79.99m, "PROD-006-PAN-BLU", 40, "Ropa y accesorios", "jeans-clasicos.jpg"),
            ("Tablet Pro", "Tablet 10.5 pulgadas con stylus incluido", 459.99m, "PROD-007-TAB-SIL", 20, "Electrónicos", "tablet-pro.jpg"),
            ("Zapatillas Running", "Zapatillas deportivas para correr", 129.99m, "PROD-008-ZAP-BLK", 35, "Deportes", "zapatillas-running.jpg"),
            ("Cafetera Express", "Cafetera automática con molinillo integrado", 299.99m, "PROD-009-CAF-BLK", 12, "Hogar", "cafetera-express.jpg"),
            ("Programación en C#", "Guía completa de programación en C# y .NET", 49.99m, "PROD-010-LIB-ESP", 80, "Libros", "programacion-csharp.jpg"),
            ("Chaqueta Deportiva", "Chaqueta impermeable para actividades al aire libre", 149.99m, "PROD-011-CHA-NAV", 28, "Ropa y accesorios", "chaqueta-deportiva.jpg"),
            ("Auriculares Bluetooth", "Auriculares inalámbricos con cancelación de ruido", 189.99m, "PROD-012-AUR-BLK", 45, "Electrónicos", "auriculares-bluetooth.jpg"),
        };

        foreach (var p in products)
        {
            if (!categoriesByName.TryGetValue(p.Category, out var category))
            {
                // La categoría no existe: se omite el producto en vez de fallar toda la siembra.
                continue;
            }

            appContext.Products.Add(new Product
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SKU = p.Sku,
                Stock = p.Stock,
                Category = category, // EF asigna CategoryId a partir de la navegación
                ImgUrl = $"{publicBaseUrl}/ProductsImages/seed/{p.ImgFileName}",
                CreationDate = DateTime.UtcNow
            });
        }

        appContext.SaveChanges();
    }
}
