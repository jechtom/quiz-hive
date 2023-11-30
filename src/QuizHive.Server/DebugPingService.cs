

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace QuizHive.Server
{
    public class DebugPingService : BackgroundService
    {
        private readonly ILogger<DebugPingService> logger;

        public DebugPingService(IServiceProvider serviceProvider, ILogger<DebugPingService> logger)
        {
            ServiceProvider = serviceProvider;
            this.logger = logger;
        }

        public IServiceProvider ServiceProvider { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                if(stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    await ExecuteTickAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process ping.");
                }
            }
        }

        private async Task ExecuteTickAsync(CancellationToken stoppingToken)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var hub = scope.ServiceProvider.GetRequiredService<IHubContext<Hubs.AppHub>>();
                var message = new Hubs.Messages.PingMessage();
                await hub.Clients.All.SendAsync(nameof(Hubs.Messages.PingMessage), message);
                logger.LogDebug("Ping {Id}", message.Id);
            }
        }
    }
}
