using Ora.GameManaging.Server.Data.Repositories;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class GameRoomLoader(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Load all rooms and players from the database into memory at application startup
            using var scope = serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<GameRoomRepository>();
            await GameHub.LoadAllRoomsAsync(repo);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
