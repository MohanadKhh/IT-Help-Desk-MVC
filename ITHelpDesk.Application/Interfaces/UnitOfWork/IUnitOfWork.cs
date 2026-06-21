using System.Threading.Tasks;
using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }
    ITicketRepository TicketRepository { get; }
    ITicketCommentRepository TicketCommentRepository { get; }
    ITicketHistoryRepository TicketHistoryRepository { get; }
    Task<int> SaveChangesAsync();
}
