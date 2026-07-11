using System;
using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetAllProducts();
    ICollection<Product> GetProductsForCategory(int categoryId);
    ICollection<Product> SearchProducts(string searchItem);
    Product? GetProductById(int productId);
    bool BuyProduct(string name, int quantity);
    bool ProductExists(int productId);
    bool ProductExists(string name);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();
}
