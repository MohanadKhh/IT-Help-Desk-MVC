using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.DTOs.Tickets;
using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application
{
    public static class TicketMapper
    {
        public static TicketDto MapToDto(this Ticket ticket)
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
                string.Empty,
                ticket.AssignedToId,
                null!,
                ticket.CreatedAt,
                ticket.UpdatedAt,
                ticket.ResolvedAt,
                null
            );
        }

        public static TicketDto MapToDto(this Ticket ticket, string createdByName, string? assignedToName)
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
                ticket.CreatedAt,
                ticket.UpdatedAt,
                ticket.ResolvedAt,
                null
            );
        }

        public static TicketDto MapToDto(this Ticket ticket, string createdByName, string? assignedToName, IEnumerable<TicketCommentDto> ticketComments)
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
                ticket.CreatedAt,
                ticket.UpdatedAt,
                ticket.ResolvedAt,
                ticketComments
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
                ticket.AssignedToId
            );
        }
    }
}