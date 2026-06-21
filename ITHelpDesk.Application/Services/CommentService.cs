using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;
using System.Net.Sockets;

namespace ITHelpDesk.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public CommentService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task<GeneralResult> CreateCommentAsync(CreateTicketCommentDto dto)
        {
            var ticket =
                await _unitOfWork.TicketRepository.GetByIdAsync(dto.TicketId);

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
            await _unitOfWork.SaveChangesAsync();

            return GeneralResult.SuccessedResult("Comment added.");
        }
        public async Task<GeneralResult<int>> EditCommentAsync(EditTicketCommentDto dto)
        {
            var comment = await _unitOfWork.TicketCommentRepository.GetByIdAsync(dto.CommentId);

            if (comment == null)
                return GeneralResult<int>.NotFound("Comment not found.");

            if (comment.Content == dto.Content)
                return GeneralResult<int>.SuccessedResult(comment.TicketId, "No changes detected.");

            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TicketCommentRepository.Update(comment);

            await _unitOfWork.SaveChangesAsync();

            return GeneralResult<int>.SuccessedResult(comment.TicketId, "Comment updated.");
        }
        public async Task<GeneralResult> DeleteCommentAsync(int commentId)
        {
            var comment =
                await _unitOfWork.TicketCommentRepository.GetByIdAsync(commentId);

            if (comment == null)
                return GeneralResult.NotFound("Comment not found.");

            _unitOfWork.TicketCommentRepository.Remove(comment);

            await _unitOfWork.SaveChangesAsync();

            return GeneralResult.SuccessedResult("Comment deleted.");
        }
        public async Task<IEnumerable<TicketCommentDto>> GetTicketCommentsAsync(int ticketId)
        {
            var ticketComments = await _unitOfWork.TicketCommentRepository.GetCommentsForTicketAsync(ticketId);

            var result = await Task.WhenAll(
                ticketComments.Select(async c => new TicketCommentDto(
                    c.CommentId,
                    await _userService.GetUserNameByIdAsync(c.CreatedById) ?? "Unknown",
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt
                )));

            return result;
        }
    }
}
