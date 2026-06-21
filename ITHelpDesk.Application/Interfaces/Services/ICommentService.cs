using ITHelpDesk.Application.DTOs.TicketComments;

namespace ITHelpDesk.Application.Interfaces.Services
{
    public interface ICommentService
    {
        Task<GeneralResult> CreateCommentAsync(CreateTicketCommentDto dto);
        Task<GeneralResult<int>> EditCommentAsync(EditTicketCommentDto dto);
        Task<GeneralResult> DeleteCommentAsync(int commentId);
        Task<IEnumerable<TicketCommentDto>> GetTicketCommentsAsync(int ticketId);
    }
}
