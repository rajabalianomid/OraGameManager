﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server;
using Ora.GameManaging.Server.Data;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using Ora.GameManaging.Server.Infrastructure.Services;
using System.Text;

// Rest of the code remains unchanged

Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         config.SetBasePath(Directory.GetCurrentDirectory());
         config.AddJsonFile("appsettings.json", optional: true);
     })
    .ConfigureWebHostDefaults((webBuilder) =>
    {
        webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001");
        webBuilder.ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;
            var jwtSecurityKey = configuration["JwtSecurityKey"];
            if (string.IsNullOrEmpty(jwtSecurityKey))
            {
                throw new InvalidOperationException("JwtSecurityKey is not configured in appsettings.json.");
            }
            var JwtSecurityKey = Encoding.ASCII.GetBytes(jwtSecurityKey);

            services.AddDbContext<GameDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging(false));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(JwtSecurityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/gameHub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddGrpc();
            services.AddSignalR();
            services.AddHostedService<GameRoomLoader>();
            //Logic
            services.AddHostedService<StalePlayerCleanupService>();
            services.AddSingleton<NotificationManager>();
            services.AddSingleton<TurnManager>();
            //Repositories
            services.AddScoped<GameRoomRepository>();
            services.AddScoped<PlayerRepository>();
            services.AddScoped<EventRepository>();
            services.AddScoped<RoomRepository>();
            //Services
            services.AddScoped<IGameRoomServices, GameRoomServices>();
            //GRPC
            services.AddSingleton<GrpcHelloService>();


            #region GRPC Clients

            var grpcServers = configuration.GetSection("GrpcServers").Get<Dictionary<string, string>>() ?? [];

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
                endpoints.MapGrpcService<GameRoomServices>();
            });
        });
        webBuilder.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5001, listenOptions =>
            {
                listenOptions.UseHttps();
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
            });
        });
    })
    .Build()
    .Run();
