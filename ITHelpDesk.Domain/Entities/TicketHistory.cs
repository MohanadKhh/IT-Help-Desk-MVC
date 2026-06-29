using ITHelpDesk.Domain.Enums;
namespace ITHelpDesk.Domain.Entities;

public class TicketHistory
{
    public int HistoryId { get; set; }
    public int TicketId { get; set; }
    public int ChangedById { get; set; }
    public TicketHistoryField FieldChanged { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }

    public Ticket? Ticket { get; set; }
}