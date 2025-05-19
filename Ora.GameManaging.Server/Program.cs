using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure;
using Ora.GameManaging.Server.Infrastructure.Proxy;

// Rest of the code remains unchanged

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
            services.AddSingleton<GrpcHelloService>();

            #region GRPC Services

            var grpcServers = config.GetSection("GrpcServers").Get<Dictionary<string, string>>() ?? new();

            foreach (var server in grpcServers)
            {
                services.AddGrpcClient<Greeter.GreeterClient>(server.Key, o =>
                {
                    o.Address = new Uri(server.Value);
                });
            }

            #endregion
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
