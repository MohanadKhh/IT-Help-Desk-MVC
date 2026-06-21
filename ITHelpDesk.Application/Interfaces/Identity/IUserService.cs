using ITHelpDesk.Application.DTOs.User;

namespace ITHelpDesk.Application.Interfaces.Identity
{
    public interface IUserService
    {
        Task<Dictionary<int, string>> GetUserNamesByIdsAsync(IEnumerable<int> userIds);
        Task<List<UserSelectItemDto>> GetAllUsersDropdownAsync();
        Task<string?> GetUserNameByIdAsync(int userId);
        Task<string?> GetEmailByUserIdAsync(int userId);
    }
}
