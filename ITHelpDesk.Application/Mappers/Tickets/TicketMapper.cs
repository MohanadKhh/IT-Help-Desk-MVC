using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.DTOs.TicketHistories;
using ITHelpDesk.Application.DTOs.Tickets;
using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application
{
    public static class TicketMapper
    {
        public static TicketDto MapToDto(this Ticket ticket, string? createdByName = null, string? assignedToName = null,
                                        List<TicketCommentDto>? ticketComments = null, List<TicketHistoryDto>? ticketHistories = null)
        {
            return new TicketDto(
                ticket.TicketId,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.Priority,
                ticket.CategoryId,
                ticket.Category?.Name ?? string.Empty,
                ticket.CreatedById,
                createdByName,
                ticket.AssignedToId,
                assignedToName,
                ticket.DueDate,
                ticket.CreatedAt,
                ticket.UpdatedAt,
                ticket.ResolvedAt,
                ticketComments,
                ticketHistories
            );
        }

        public static EditTicketDto MapToEditDto(this Ticket ticket)
        {
            return new EditTicketDto(
                ticket.TicketId,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.Priority,
                ticket.CategoryId,
                ticket.AssignedToId,
                ticket.DueDate
            );
        }
    }
}