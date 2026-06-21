using ITHelpDesk.Application.Interfaces.Repositories;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Infrastructure.Data;
using ITHelpDesk.Infrastructure.Repositories;

namespace ITHelpDesk.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICategoryRepository CategoryRepository => new CategoryRepository(_context);
    public ITicketRepository TicketRepository => new TicketRepository(_context);
    public ITicketCommentRepository TicketCommentRepository => new TicketCommentRepository(_context);
    public ITicketHistoryRepository TicketHistoryRepository => new TicketHistoryRepository(_context);

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
