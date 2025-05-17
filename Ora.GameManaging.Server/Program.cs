using Ora.GameManaging.Server;
using Ora.GameManaging.Server.Infrastructure;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseUrls("http://localhost:5000");
        webBuilder.ConfigureServices(services =>
        {
            services.AddSignalR();
            services.AddSingleton<NotificationManager>();
            services.AddSingleton<TurnManager>();
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
