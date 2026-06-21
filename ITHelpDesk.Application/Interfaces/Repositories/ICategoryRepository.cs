using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<IEnumerable<Category?>> GetAllWithTicketsAsync();
}
