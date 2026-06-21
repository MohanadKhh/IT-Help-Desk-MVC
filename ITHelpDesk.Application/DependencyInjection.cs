using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ITHelpDesk.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ICategoryService, CategoryService>();
            return services;
        }
    }
}