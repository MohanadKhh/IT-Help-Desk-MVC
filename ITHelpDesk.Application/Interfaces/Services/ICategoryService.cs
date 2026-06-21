using ITHelpDesk.Application.DTOs.Categories;

namespace ITHelpDesk.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<GeneralResult<List<CategoryDto>>> GetAllAsync();
        Task<List<CategorySelectItem>> GetCategoriesDropdownAsync();
    }
}
