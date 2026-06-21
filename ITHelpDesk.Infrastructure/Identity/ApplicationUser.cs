using Microsoft.AspNetCore.Identity;

namespace ITHelpDesk.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
}
