using ITHelpDesk.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ITHelpDesk.Infrastructure.BackgroundJobs
{
    public class SlaBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SlaBackgroundJob> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public SlaBackgroundJob(IServiceProvider serviceProvider, ILogger<SlaBackgroundJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SLA background job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var slaCheckService = scope.ServiceProvider.GetRequiredService<ISlaCheckService>();
                    await slaCheckService.RunCheckAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during SLA check.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
