using ITHelpDesk.Application.DTOs.Authentications;

namespace ITHelpDesk.Application.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<GeneralResult> LoginAsync(LoginDto dto);
        Task<GeneralResult> RegisterAsync(RegisterDto dto, string confirmationBaseUrl);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task LogoutAsync();
    }
}
