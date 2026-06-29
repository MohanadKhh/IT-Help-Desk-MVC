using FluentValidation;
using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;
using System.Net.Sockets;

namespace ITHelpDesk.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHistoryService _historyService;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<CreateTicketCommentDto> _createValidator;
        private readonly IValidator<EditTicketCommentDto> _editValidator;

        public CommentService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
                              IUserService userService, IHistoryService historyService,
                              IValidator<CreateTicketCommentDto> createValidator,
                              IValidator<EditTicketCommentDto> editValidator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _historyService = historyService;
            _userService = userService;
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        public async Task<GeneralResult> CreateCommentAsync(CreateTicketCommentDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                Dictionary<string, List<Error>> errors = validationResult.ToError();
                return GeneralResult.FailedResult(errors);
            }

            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(dto.TicketId);

            if (ticket == null)
                return GeneralResult.NotFound("Ticket not found.");

            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
                return GeneralResult.FailedResult("User is not authenticated.");

            var comment = new TicketComment
            {
                TicketId = dto.TicketId,
                Content = dto.Content,
                CreatedById = _currentUserService.UserId.Value,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TicketCommentRepository.AddAsync(comment);
            await _historyService.LogHistoryAsync(ticket, TicketHistoryField.CommentAdded);

            await _unitOfWork.SaveChangesAsync();

            return GeneralResult.SuccessedResult("Comment added.");
        }

        public async Task<GeneralResult<int>> EditCommentAsync(EditTicketCommentDto dto)
        {
            var validationResult = await _editValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                Dictionary<string, List<Error>> errors = validationResult.ToError();
                return GeneralResult<int>.FailedResult(errors);
            }

            var comment = await _unitOfWork.TicketCommentRepository.GetCommentWithTicketByIdAsync(dto.CommentId);
            if (comment == null)
                return GeneralResult<int>.NotFound("Comment not found.");

            if (comment.CreatedById != _currentUserService.UserId)
            {
                return GeneralResult<int>.FailedResult("You do not have permission to edit this ticket.");
            }

            if (comment.Content == dto.Content)
                return GeneralResult<int>.SuccessedResult(comment.TicketId, "No changes detected.");

            await _historyService.LogHistoryAsync(comment.Ticket!, TicketHistoryField.CommentEdited, comment.Content, dto.Content);

            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TicketCommentRepository.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            return GeneralResult<int>.SuccessedResult(comment.TicketId, "Comment updated.");
        }

        public async Task<GeneralResult> DeleteCommentAsync(int commentId)
        {
            var comment = await _unitOfWork.TicketCommentRepository.GetCommentWithTicketByIdAsync(commentId);
            if (comment == null)
                return GeneralResult.NotFound("Comment not found.");

            if (!_currentUserService.IsAdmin && comment.CreatedById != _currentUserService.UserId)
            {
                return GeneralResult.FailedResult("You do not have permission to edit this ticket.");
            }

            _unitOfWork.TicketCommentRepository.Remove(comment);

            await _historyService.LogHistoryAsync(comment.Ticket!, TicketHistoryField.CommentDeleted);
            await _unitOfWork.SaveChangesAsync();


            return GeneralResult.SuccessedResult("Comment deleted.");
        }

        public async Task<List<TicketCommentDto>> GetTicketCommentsAsync(int ticketId)
        {
            var ticketComments = await _unitOfWork.TicketCommentRepository.GetCommentsForTicketAsync(ticketId);
            
            var userIds = ticketComments.Select(c => c.CreatedById).Distinct().ToList();
            var userNames = await _userService.GetUserNamesByIdsAsync(userIds);

            var result = ticketComments.Select(c => new TicketCommentDto(
                    c.CommentId,
                    c.CreatedById,
                    userNames.GetValueOrDefault(c.CreatedById, "Unknown"),
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt
                ));

            return result.ToList();
        }
    }
}
