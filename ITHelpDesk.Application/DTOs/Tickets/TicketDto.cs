using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Domain.Enums;

namespace ITHelpDesk.Application.DTOs.Tickets;

public record TicketDto(
    int TicketId,
    string Title,
    string Description,
    TicketStatus Status,
    string Priority,
    int CategoryId,
    string CategoryName,
    int CreatedById,
    string CreatedByName,
    int? AssignedToId,
    string? AssignedToName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ResolvedAt,
    IEnumerable<TicketCommentDto>? Comments
);

public record CreateTicketDto(
    string Title,
    string Description,
    string Priority,
    int CategoryId,
    int? AssignedToId
);

public record EditTicketDto(
    int TicketId,
    string Title,
    string Description,
    TicketStatus Status,
    string Priority,
    int CategoryId,
    int? AssignedToId
);
