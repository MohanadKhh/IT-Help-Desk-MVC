using ITHelpDesk.Application.DTOs.Categories;
using ITHelpDesk.Application.DTOs.User;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITHelpDesk.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllWithTicketsAsync();

            var dtos = categories
                .Select(c => new CategoryDto(
                    c.CategoryId,
                    c.Name,
                    c.Description,
                    c.Tickets?.Select(t => t.MapToDto()).ToList() ?? new()
                ))
                .ToList();

            return GeneralResult<List<CategoryDto>>.SuccessedResult(dtos);
        }

        public async Task<List<CategorySelectItem>> GetCategoriesDropdownAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return categories
                .Select(c => new CategorySelectItem(c.CategoryId, c.Name))
                .ToList();
        }

    }
}
