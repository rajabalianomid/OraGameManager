using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Server;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseUrls("http://localhost:5000");
        webBuilder.ConfigureServices(services =>
        {
            services.AddDbContext<GameDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddSignalR();
            services.AddHostedService<GameRoomLoader>();
            services.AddHostedService<StalePlayerCleanupService>();
            services.AddSingleton<NotificationManager>();
            services.AddSingleton<TurnManager>();
            services.AddScoped<GameRoomRepository>();
            services.AddScoped<PlayerRepository>();
            services.AddScoped<EventRepository>();
        });

        webBuilder.Configure(app =>
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("/gamehub");
            });
        });
    })
    .Build()
    .Run();
