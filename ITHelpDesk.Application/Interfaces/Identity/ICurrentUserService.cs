namespace ITHelpDesk.Application.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
    }
}
