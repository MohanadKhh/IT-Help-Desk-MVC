using ITHelpDesk.Application.DTOs.Tickets;

namespace ITHelpDesk.Application.DTOs.Categories;

public record CategoryDto(
    int CategoryId,
    string Name,
    string? Description,
    ICollection<TicketDto> ticketDtos
);

public record CreateCategoryDto(
    string Name,
    string? Description
);

public record UpdateCategoryDto(
    int CategoryId,
    string Name,
    string? Description
);
