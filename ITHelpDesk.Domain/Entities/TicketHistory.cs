namespace ITHelpDesk.Domain.Entities;

public class TicketHistory
{
    public int HistoryId { get; set; }
    public int TicketId { get; set; }
    public int ChangedById { get; set; }
    public string FieldChanged { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }

    public Ticket? Ticket { get; set; }
}