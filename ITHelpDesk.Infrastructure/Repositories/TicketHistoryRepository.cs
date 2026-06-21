using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Repositories;

public class TicketHistoryRepository : Repository<TicketHistory, int>, ITicketHistoryRepository
{
    public TicketHistoryRepository(AppDbContext context) : base(context)
    {
    }
}
