using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category, int>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Category?>> GetAllWithTicketsAsync()
    {
        return await _context.Categories
        .Include(c => c.Tickets)
        .ToListAsync();
    }
}
