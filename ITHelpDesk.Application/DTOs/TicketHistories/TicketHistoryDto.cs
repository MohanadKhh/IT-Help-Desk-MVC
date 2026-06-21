namespace ITHelpDesk.Application.DTOs.TicketHistories;

public record TicketHistoryDto(
    int HistoryId,
    int TicketId,
    int ChangedById,
    string FieldChanged,
    string? OldValue,
    string? NewValue,
    DateTime ChangedAt
);

public record CreateTicketHistoryDto(
    int TicketId,
    int ChangedById,
    string FieldChanged,
    string? OldValue,
    string? NewValue
);
