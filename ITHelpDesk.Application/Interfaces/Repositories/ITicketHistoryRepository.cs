using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Interfaces.Repositories;

public interface ITicketHistoryRepository : IRepository<TicketHistory, int>
{
}
