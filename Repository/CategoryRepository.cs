using ApiEcommerce.Repository.IRepository;
using ApiEcommerce.Models;

namespace ApiEcommerce.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool CategoryExists(int categoryId)
    {
        return _dbContext.Categories.Any(c => c.Id == categoryId);
    }

    public bool CategoryExists(string categoryName)
    {
        return _dbContext.Categories.Any(c => c.Name.ToLower().Trim() == categoryName.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _dbContext.Categories.Add(category);
        return Save();
    }

    public bool DeleteCategory(Category category)
    {
        _dbContext.Categories.Remove(category);
        return Save();
    }

    public ICollection<Category> GetAllCategories()
    {
        return _dbContext.Categories.OrderBy(c => c.Name).ToList();
    }

    public Category? GetCategoryById(int categoryId)
    {
        return _dbContext.Categories.FirstOrDefault(c => c.Id == categoryId);
    }

    public bool Save()
    {
        return _dbContext.SaveChanges() >= 0 ? true : false;
    }

    public bool UpdateCategory(Category category)
    {
        category.CreationDate = DateTime.Now;
        _dbContext.Categories.Update(category);
        return Save();
    }
}