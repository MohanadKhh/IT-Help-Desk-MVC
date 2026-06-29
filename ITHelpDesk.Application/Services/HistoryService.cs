using ITHelpDesk.Application.DTOs.TicketHistories;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Interfaces.UnitOfWork;
using ITHelpDesk.Domain.Entities;
using ITHelpDesk.Domain.Enums;

namespace ITHelpDesk.Application.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public HistoryService(IUnitOfWork unitOfWork, IUserService userService, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _currentUserService = currentUserService;
        }

        public async Task LogHistoryAsync(Ticket ticket, TicketHistoryField field, string? oldValue = null, string? newValue = null)
        {
            if (_currentUserService.UserId is null)
            {
                throw new InvalidOperationException("Current user ID is not available.");
            }

            var history = new TicketHistory
            {
                Ticket = ticket,
                ChangedById = _currentUserService.UserId.Value,
                FieldChanged = field,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedAt = DateTime.UtcNow
            };

            await _unitOfWork.TicketHistoryRepository.AddAsync(history);
        }

        public async Task<List<TicketHistoryDto>> GetTicketHistoriesAsync(int ticketId)
        {
            var histories = await _unitOfWork.TicketHistoryRepository.GetByTicketIdAsync(ticketId);

            var userIds = histories.Select(h => h.ChangedById).Distinct().ToList();
            var userNames = await _userService.GetUserNamesByIdsAsync(userIds);

            return histories
                .Select(h => new TicketHistoryDto(
                    h.HistoryId,
                    userNames.GetValueOrDefault(h.ChangedById, "Unknown"),
                    h.FieldChanged,
                    h.OldValue,
                    h.NewValue,
                    h.ChangedAt
                ))
                .ToList();
        }
    }
}
