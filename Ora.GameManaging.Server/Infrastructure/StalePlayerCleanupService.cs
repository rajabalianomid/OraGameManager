using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ora.GameManaging.Server.Data.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

public class StalePlayerCleanupService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var playerRepo = scope.ServiceProvider.GetRequiredService<PlayerRepository>();
                await playerRepo.RemoveStalePlayersAsync(DateTime.UtcNow.AddMinutes(-5));
            }
            await Task.Delay(_interval, stoppingToken);
        }
    }
}