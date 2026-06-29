using ITHelpDesk.Application.DTOs.TicketHistories;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;

namespace ITHelpDesk.Application.Interfaces.Services
{
    public interface IHistoryService
    {
        Task LogHistoryAsync(Ticket ticket, TicketHistoryField field, string? oldValue = null, string? newValue = null);
        Task<List<TicketHistoryDto>> GetTicketHistoriesAsync(int ticketId);
    }
}
