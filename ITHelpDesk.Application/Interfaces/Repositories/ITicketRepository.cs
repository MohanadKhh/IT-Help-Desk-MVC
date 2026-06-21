using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Interfaces.Repositories;

public interface ITicketRepository : IRepository<Ticket, int>
{
    Task<IEnumerable<Ticket>> GetTicketsWithCategoryAsync();
    Task<Ticket?> GetTicketByIdWithIncludesAsync(int id);
}
