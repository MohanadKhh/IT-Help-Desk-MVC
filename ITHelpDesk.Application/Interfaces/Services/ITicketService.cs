using ITHelpDesk.Application.DTOs.Tickets;

namespace ITHelpDesk.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<GeneralResult> CreateTicketAsync(CreateTicketDto createTicketDto);
        Task<GeneralResult> DeleteTicketAsync(int ticketId);
        Task<GeneralResult<IEnumerable<TicketDto>>> GetAllTicketsAsync();
        Task<GeneralResult<TicketDto>> GetTicketByIdAsync(int ticketId);
        Task<GeneralResult<EditTicketDto>> GetTicketForEditAsync(int ticketId);
        Task<GeneralResult> UpdateTicketAsync(EditTicketDto updateTicketDto);
    }
}