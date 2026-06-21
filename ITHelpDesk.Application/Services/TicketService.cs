using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.DTOs.Tickets;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;

namespace ITHelpDesk.Application.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUserService;

    public TicketService(IUnitOfWork unitOfWork, IUserService userService,
                        IEmailService emailService,
                        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _emailService = emailService;
        _currentUserService = currentUserService;
    }

    public async Task<GeneralResult> CreateTicketAsync(CreateTicketDto createTicketDto)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            return GeneralResult.FailedResult("User is not authenticated.");

        var ticket = new Ticket
        {
            Title = createTicketDto.Title,
            Description = createTicketDto.Description,
            Priority = createTicketDto.Priority,
            CategoryId = createTicketDto.CategoryId,
            CreatedById = _currentUserService.UserId.Value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TicketRepository.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        return GeneralResult<TicketDto>.SuccessedResult(ticket.MapToDto(), "Ticket created successfully.");
    }

    public async Task<GeneralResult> UpdateTicketAsync(EditTicketDto editTicketDto)
    {
        var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(editTicketDto.TicketId);

        if (ticket == null)
        {
            return GeneralResult.NotFound($"Ticket {editTicketDto.TicketId} not found.");
        }

        var userEmail = await _userService.GetEmailByUserIdAsync(ticket.CreatedById);
        if (editTicketDto.Status == Domain.Enums.TicketStatus.Resolved
            && ticket.Status != Domain.Enums.TicketStatus.Resolved
            && !string.IsNullOrEmpty(userEmail))
        {
            var body = $@"
                        <h2>Ticket Resolved</h2>

                        <p>Hello,</p>

                        <p>Your support ticket has been successfully resolved.</p>

                        <p>
                        <strong>Ticket ID:</strong> {ticket.TicketId}<br />
                        <strong>Title:</strong> {ticket.Title}<br />
                        <strong>Status:</strong> Resolved
                        </p>

                        <p>
                        The support team has completed the required actions and marked the ticket as resolved.
                        If there any other issues, please create a new ticket for further assistance.
                        </p>

                        <p>
                        Thank you for using the IT Help Desk system.
                        </p>

                        <p>
                        Best regards,<br />
                        IT Help Desk Team
                        </p>";

            ticket.ResolvedAt = DateTime.UtcNow;
            await _emailService.SendEmailAsync(userEmail, $"Ticket {ticket.TicketId} is Resolved", body);
        }

        if (HasChanges(ticket, editTicketDto))
        {
            ticket.Title = editTicketDto.Title;
            ticket.Description = editTicketDto.Description;
            ticket.Status = editTicketDto.Status;
            ticket.Priority = editTicketDto.Priority;
            ticket.CategoryId = editTicketDto.CategoryId;
            ticket.AssignedToId = editTicketDto.AssignedToId;
            ticket.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            return GeneralResult.FailedResult("No changes detected to update.");
        }

        _unitOfWork.TicketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        return GeneralResult<TicketDto>.SuccessedResult(ticket.MapToDto(), "Ticket updated successfully.");
    }

    public async Task<GeneralResult> DeleteTicketAsync(int ticketId)
    {
        var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);

        if (ticket == null)
        {
            return GeneralResult.NotFound($"Ticket {ticketId} not found.");
        }

        _unitOfWork.TicketRepository.Remove(ticket);
        await _unitOfWork.SaveChangesAsync();

        return GeneralResult.SuccessedResult("Ticket deleted successfully.");
    }

    public async Task<GeneralResult<TicketDto>> GetTicketByIdAsync(int ticketId)
    {
        var ticket = await _unitOfWork.TicketRepository.GetTicketByIdWithIncludesAsync(ticketId);
        if (ticket == null)
        {
            return GeneralResult<TicketDto>.NotFound($"Ticket {ticketId} not found.");
        }

        var createdByName = await _userService.GetUserNameByIdAsync(ticket.CreatedById);
        var assignedToName = ticket.AssignedToId.HasValue
                            ? await _userService.GetUserNameByIdAsync(ticket.AssignedToId.Value)
                            : null;

        IEnumerable<TicketCommentDto> commentsDtos = Enumerable.Empty<TicketCommentDto>(); ;
        if (ticket.TicketComments?.Any() == true)
        {
            commentsDtos = (await Task.WhenAll(
                ticket.TicketComments.Select(async c => new TicketCommentDto(
                    c.CommentId,
                    await _userService.GetUserNameByIdAsync(c.CreatedById) ?? "Unknown",
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt
                ))
            )).ToList();
        }

        var ticketDto = ticket.MapToDto(
                    createdByName: createdByName ?? "Unknown",
                    assignedToName: assignedToName ?? null,
                    ticketComments: commentsDtos
                );

        return GeneralResult<TicketDto>.SuccessedResult(ticketDto, $"Ticket {ticketId} retrieved successfully.");
    }

    public async Task<GeneralResult<EditTicketDto>> GetTicketForEditAsync(int ticketId)
    {
        var ticket = await _unitOfWork.TicketRepository.GetTicketByIdWithIncludesAsync(ticketId);
        if (ticket == null)
        {
            return GeneralResult<EditTicketDto>.NotFound($"Ticket {ticketId} not found.");
        }

        var editTicketDto = ticket.MapToEditDto();
        return GeneralResult<EditTicketDto>.SuccessedResult(editTicketDto, $"Ticket {ticketId} retrieved successfully for editing.");
    }

    public async Task<GeneralResult<IEnumerable<TicketDto>>> GetAllTicketsAsync()
    {
        var tickets = await _unitOfWork.TicketRepository.GetTicketsWithCategoryAsync();

        var creatorIds = tickets
            .Select(t => t.CreatedById)
            .ToList();

        var assigneeIds = tickets
            .Where(t => t.AssignedToId != null)
            .Select(t => t.AssignedToId!.Value)
            .ToList();

        var userIds = creatorIds.Union(assigneeIds).ToList();
        var users = await _userService.GetUserNamesByIdsAsync(userIds);

        var ticketDtos = tickets.Select(ticket => ticket.MapToDto(
                    createdByName: users.GetValueOrDefault(ticket.CreatedById, "Unknown"),
                    assignedToName: ticket.AssignedToId.HasValue
                                    ? users.GetValueOrDefault(ticket.AssignedToId.Value, "Unassigned")
                                    : null));

        return GeneralResult<IEnumerable<TicketDto>>.SuccessedResult(ticketDtos, "All tickets retrieved successfully.");
    }

    private static bool HasChanges(Ticket ticket, EditTicketDto dto)
    {
        return ticket.Title != dto.Title ||
               ticket.Description != dto.Description ||
               ticket.Status != dto.Status ||
               ticket.Priority != dto.Priority ||
               ticket.CategoryId != dto.CategoryId ||
               ticket.AssignedToId != dto.AssignedToId;
    }
}
