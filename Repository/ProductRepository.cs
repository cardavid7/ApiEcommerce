using System;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool BuyProduct(string name, int quantity)
    {
        if(string.IsNullOrWhiteSpace(name) || quantity <= 0)
        {
            return false;
        }
        var product = _dbContext.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        if(product == null || product.Stock < quantity)
        {
            return false;
        }
        product.Stock -= quantity;
        _dbContext.Products.Update(product);
        return true;
    }

    public bool CreateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        product.CreationDate = DateTime.Now;
        product.UpdateDate = DateTime.Now;
        _dbContext.Products.Add(product);
        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        _dbContext.Products.Remove(product);
        return Save();
    }

    public ICollection<Product> GetAllProducts()
    {
        return _dbContext.Products.OrderBy(p => p.Name).ToList();
    }

    public Product? GetProductById(int productId)
    {
        if (productId <= 0)
        {
            return null;
        }
        return _dbContext.Products.FirstOrDefault(p => p.Id == productId);
    }

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        if (categoryId <= 0)
        {
            return new List<Product>();
        }
        return _dbContext.Products.Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
    }

    public bool ProductExists(int productId)
    {
        if (productId <= 0)
        {
            return false;
        }
        return _dbContext.Products.Any(p => p.Id == productId);
    }

    public bool ProductExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        return _dbContext.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool Save()
    {
        return _dbContext.SaveChanges() >= 0;
    }

    public ICollection<Product> SearchProducts(string searchItem)
    {
        IQueryable<Product> query = _dbContext.Products;
        if (!string.IsNullOrWhiteSpace(searchItem))
        {
            query = query.Where(p => p.Name.ToLower().Trim().Contains(searchItem.ToLower().Trim()));
        }
        return query.OrderBy(p => p.Name).ToList();
    }

    public bool UpdateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        product.UpdateDate = DateTime.Now;
        _dbContext.Products.Update(product);
        return Save();
    }
}
