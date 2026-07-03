namespace Repository.IRepository;

public interface ICategoryRepository
{
    ICollection<Category> GetAllCategories();
    Category GetCategoryById(int categoryId);
    bool CategoryExists(int categoryId);
    bool CategoryExists(string categoryName);
    bool CreateCategory(Category category);
    bool UpdateCategory(Category category);
    bool DeleteCategory(Category category);
    bool Save();
}