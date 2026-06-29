using FluentValidation;
using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.DTOs.TicketHistories;
using ITHelpDesk.Application.DTOs.Tickets;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;
using ITHelpDesk.Application.Common.Helpers;

namespace ITHelpDesk.Application.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ICommentService _commentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHistoryService _historyService;
    private readonly IValidator<CreateTicketDto> _createValidator;
    private readonly IValidator<EditTicketDto> _editValidator;

    public TicketService(IUnitOfWork unitOfWork, IUserService userService,
                        IEmailService emailService,
                        ICommentService commentService,
                        ICurrentUserService currentUserService,
                        IHistoryService historyService,
                        IValidator<CreateTicketDto> createValidator,
                        IValidator<EditTicketDto> editValidator)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _emailService = emailService;
        _commentService = commentService;
        _historyService = historyService;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _editValidator = editValidator;
    }

    public async Task<GeneralResult> CreateTicketAsync(CreateTicketDto createTicketDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createTicketDto);
        if (!validationResult.IsValid)
        {
            Dictionary<string, List<Error>> errors = validationResult.ToError();
            return GeneralResult.FailedResult(errors);
        }

        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            return GeneralResult.FailedResult("User is not authenticated.");

        var dueDate = createTicketDto.DueDate.HasValue
                        ? DateHelper.ToUtcTime(createTicketDto.DueDate.Value)
                        : createTicketDto.Priority switch
                        {
                            "Critical" => DateTime.UtcNow.AddHours(4),
                            "High" => DateTime.UtcNow.AddHours(24),
                            "Medium" => DateTime.UtcNow.AddDays(3),
                            "Low" => DateTime.UtcNow.AddDays(7),
                            _ => DateTime.UtcNow.AddDays(3)
                        };

        var ticket = new Ticket
        {
            Title = createTicketDto.Title,
            Description = createTicketDto.Description,
            Priority = createTicketDto.Priority,
            CategoryId = createTicketDto.CategoryId,
            CreatedById = _currentUserService.UserId.Value,
            AssignedToId = createTicketDto.AssignedToId,
            CreatedAt = DateTime.UtcNow,
            DueDate = dueDate,
        };

    await _unitOfWork.TicketRepository.AddAsync(ticket);
    await _historyService.LogHistoryAsync(ticket, TicketHistoryField.TicketCreated);

    await _unitOfWork.SaveChangesAsync();

        return GeneralResult<TicketDto>.SuccessedResult(ticket.MapToDto(), "Ticket created successfully.");
    }

    public async Task<GeneralResult> UpdateTicketAsync(EditTicketDto editTicketDto)
    {
        var validationResult = await _editValidator.ValidateAsync(editTicketDto);
        if (!validationResult.IsValid)
        {
            Dictionary<string, List<Error>> errors = validationResult.ToError();
            return GeneralResult.FailedResult(errors);
        }

        var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(editTicketDto.TicketId);

        if (ticket == null)
        {
            return GeneralResult.NotFound($"Ticket {editTicketDto.TicketId} not found.");
        }

        if (!_currentUserService.IsAdmin && ticket.CreatedById != _currentUserService.UserId)
        {
            return GeneralResult.FailedResult("You do not have permission to edit this ticket.");
        }

        var userEmail = await _userService.GetEmailByUserIdAsync(ticket.CreatedById);
        if (editTicketDto.Status == TicketStatus.Resolved
            && ticket.Status != TicketStatus.Resolved
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

        var changes = GetChanges(ticket, editTicketDto);
        if (changes.Count > 0)
        {
            foreach (var change in changes)
            {
                string? oldValue = change switch
                {
                    "Title" => ticket.Title,
                    "Description" => ticket.Description,
                    "Status" => ticket.Status.ToString(),
                    "Priority" => ticket.Priority.ToString(),
                    "Category" => ticket.CategoryId.ToString(),
                    "AssignedTo" => ticket.AssignedToId.HasValue ? _userService.GetUserNameByIdAsync(ticket.AssignedToId.Value).Result : null,
                    "DueDate" => ticket.DueDate.ToString(),
                    _ => null
                };
                string? newValue = change switch
                {
                    "Title" => editTicketDto.Title,
                    "Description" => editTicketDto.Description,
                    "Status" => editTicketDto.Status.ToString(),
                    "Priority" => editTicketDto.Priority.ToString(),
                    "Category" => editTicketDto.CategoryId.ToString(),
                    "AssignedTo" => editTicketDto.AssignedToId.HasValue ? _userService.GetUserNameByIdAsync(editTicketDto.AssignedToId.Value).Result : null,
                    "DueDate" => DateHelper.ToUtcTime(editTicketDto.DueDate).ToString(),
                    _ => null
                };
                await _historyService.LogHistoryAsync(ticket, Enum.Parse<TicketHistoryField>(change), oldValue, newValue);
            }

            ticket.Title = editTicketDto.Title;
            ticket.Description = editTicketDto.Description;
            ticket.Status = editTicketDto.Status;
            ticket.Priority = editTicketDto.Priority;
            ticket.CategoryId = editTicketDto.CategoryId;
            ticket.AssignedToId = editTicketDto.AssignedToId;
            ticket.DueDate = DateHelper.ToUtcTime(editTicketDto.DueDate);
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

        if (!_currentUserService.IsAdmin && ticket.CreatedById != _currentUserService.UserId)
        {
            return GeneralResult.FailedResult("You do not have permission to delete this ticket.");
        }

        _unitOfWork.TicketRepository.Remove(ticket);
        await _historyService.LogHistoryAsync(ticket, TicketHistoryField.TicketDeleted);

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

        List<TicketCommentDto> commentsDtos = await _commentService.GetTicketCommentsAsync(ticketId);
        List<TicketHistoryDto> historiesDtos = await _historyService.GetTicketHistoriesAsync(ticketId);

        var ticketDto = ticket.MapToDto(
                    createdByName: createdByName ?? "Unknown",
                    assignedToName: assignedToName ?? null,
                    ticketComments: commentsDtos,
                    ticketHistories: historiesDtos
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

        if (!_currentUserService.IsAdmin && ticket.CreatedById != _currentUserService.UserId)
        {
            return GeneralResult<EditTicketDto>.FailedResult("You do not have permission to edit this ticket.");
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


    // Private helper
    private static List<string> GetChanges(Ticket ticket, EditTicketDto dto)
    {
        var changes = new List<string>();

        if (ticket.Title != dto.Title)
            changes.Add("Title");

        if (ticket.Description != dto.Description)
            changes.Add("Description");

        if (ticket.Status != dto.Status)
            changes.Add("Status");

        if (ticket.Priority != dto.Priority)
            changes.Add("Priority");

        if (ticket.CategoryId != dto.CategoryId)
            changes.Add("Category");

        if (ticket.AssignedToId != dto.AssignedToId)
            changes.Add("AssignedTo");

        if (ticket.DueDate != DateHelper.ToUtcTime(dto.DueDate))
            changes.Add("DueDate");

        return changes;
    }
}
