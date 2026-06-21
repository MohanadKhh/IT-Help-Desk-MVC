using ITHelpDesk.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ITHelpDesk.Infrastructure.Services.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public int? UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.NameIdentifier);

                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Name);

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User
                .Identity?.IsAuthenticated ?? false;
    }
}
