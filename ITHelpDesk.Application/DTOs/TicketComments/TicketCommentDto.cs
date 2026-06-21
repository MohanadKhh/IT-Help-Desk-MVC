namespace ITHelpDesk.Application.DTOs.TicketComments;

public record TicketCommentDto(
    int CommentId,
    string CreatedByName,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTicketCommentDto(
    int TicketId,
    string Content
);

public record EditTicketCommentDto(
    int CommentId,
    string Content
);