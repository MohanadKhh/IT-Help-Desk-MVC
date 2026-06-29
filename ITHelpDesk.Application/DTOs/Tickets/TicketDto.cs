using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.DTOs.TicketHistories;
using ITHelpDesk.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
    string? CreatedByName,
    int? AssignedToId,
    string? AssignedToName,
    DateTime DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ResolvedAt,
    List<TicketCommentDto>? Comments,
    List<TicketHistoryDto>? Histories
);

public record CreateTicketDto(
    string Title,
    string Description,
    string Priority,
    int CategoryId,
    int? AssignedToId,
    DateTime? DueDate
);

public record EditTicketDto(
    int TicketId,
    string Title,
    string Description,
    TicketStatus Status,
    string Priority,
    int CategoryId,
    int? AssignedToId,
    [property: DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    DateTime DueDate
);
