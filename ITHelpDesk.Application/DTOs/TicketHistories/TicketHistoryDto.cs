using ITHelpDesk.Domain.Enums;

namespace ITHelpDesk.Application.DTOs.TicketHistories;

public record TicketHistoryDto(
    int HistoryId,
    string ChangedByName,
    TicketHistoryField FieldChanged,
    string? OldValue,
    string? NewValue,
    DateTime ChangedAt
);
