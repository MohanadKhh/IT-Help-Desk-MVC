namespace ITHelpDesk.Domain.Entities;

public class TicketComment
{
    public int CommentId { get; set; }
    public int TicketId { get; set; }
    public int CreatedById { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
