using ITHelpDesk.Domain.Enums;

namespace ITHelpDesk.Domain.Entities;

public class Ticket
{
    public int TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public string Priority { get; set; } = "Medium";
    public int CategoryId { get; set; }
    public int CreatedById { get; set; }
    public int? AssignedToId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<TicketComment>? TicketComments { get; set; } = new HashSet<TicketComment>();
    public virtual ICollection<TicketHistory>? TicketHistories { get; set; } = new HashSet<TicketHistory>();
}
