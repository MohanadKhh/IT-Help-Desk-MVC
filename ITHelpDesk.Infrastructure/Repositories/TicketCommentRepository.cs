using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure.Repositories;

public class TicketCommentRepository : Repository<TicketComment, int>, ITicketCommentRepository
{
    public TicketCommentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<TicketComment?> GetCommentWithTicketByIdAsync(int commentId)
    {
        return await _context.TicketComments
            .Include(c => c.Ticket)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task<List<TicketComment>> GetCommentsForTicketAsync(int ticketId)
    {
        return await _context.TicketComments
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}
