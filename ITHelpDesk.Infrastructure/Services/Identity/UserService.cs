using ITHelpDesk.Application.DTOs.User;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.Infrastructure
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Dictionary<int, string>> GetUserNamesByIdsAsync(
            IEnumerable<int> userIds)
        {
            return await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToDictionaryAsync(
                    keySelector: u => u.Id,
                    elementSelector: u => u.FullName ?? "Unknown");
        }
        public async Task<List<UserSelectItemDto>> GetAllUsersDropdownAsync()
        {
            return await _userManager.Users
                .Select(u => new UserSelectItemDto(
                    u.Id,
                    u.FullName ?? u.UserName ?? "Unknown"
                ))
                .ToListAsync();
        }

        public async Task<string?> GetUserNameByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.UserName;
        }

        public async Task<string?> GetEmailByUserIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.Email;
        }

        public async Task<List<string>> GetAdminEmailsAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email!)
                .ToList();
        }
    }
}