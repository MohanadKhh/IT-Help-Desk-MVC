using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;
using ITHelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Repositories;

public class TicketRepository : Repository<Ticket, int>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ticket>> GetTicketsWithCategoryAsync()
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketByIdWithIncludesAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.TicketComments)
            .Include(t => t.TicketHistories)
            .FirstOrDefaultAsync(t => t.TicketId == id);
    }

    public async Task<List<Ticket>> GetTicketsNeedingSlaCheckAsync()
    {
        return await _context.Tickets
            .Where(t => (t.Status != TicketStatus.Resolved
                     && t.Status != TicketStatus.Closed
                     && (!t.WarningEmailSent || !t.BreachEmailSent))
                     || (t.AssignedToId == null && !t.UnassignedReminderSent)
                    )
            .ToListAsync();
    }
}
