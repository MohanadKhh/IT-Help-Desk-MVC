using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Interfaces.Repositories;

public interface ITicketCommentRepository : IRepository<TicketComment, int>
{
    Task<TicketComment?> GetCommentWithTicketByIdAsync(int commentId);
    Task<List<TicketComment>> GetCommentsForTicketAsync(int ticketId);
}
