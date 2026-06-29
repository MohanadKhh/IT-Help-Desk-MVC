using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ITHelpDesk.Infrastructure.Repositories;

public class TicketHistoryRepository : Repository<TicketHistory, int>, ITicketHistoryRepository
{
    public TicketHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<List<TicketHistory>> GetByTicketIdAsync(int ticketId)
    {
        return await _context.TicketHistories
            .Where(h => h.TicketId == ticketId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}