using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Interfaces.Repositories;

public interface ITicketCommentRepository : IRepository<TicketComment, int>
{
    Task<List<TicketComment>> GetCommentsForTicketAsync(int ticketId);
}
