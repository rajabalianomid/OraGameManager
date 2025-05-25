using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ora.GameManaging.Mafia.Data;
using Ora.GameManaging.Mafia.Data.Repositories;
using Ora.GameManaging.Mafia.Infrastructure;
using Ora.GameManaging.Mafia.Infrastructure.Services;
using Ora.GameManaging.Mafia.Infrastructure.Services.Proxy;
using Ora.GameManaging.Mafia.Protos;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSecurityKeySection = builder.Configuration.GetSection("JwtSecurityKey");
if (string.IsNullOrEmpty(jwtSecurityKeySection?.Value))
{
    throw new InvalidOperationException("JwtSecurityKey is not configured in appsettings.json.");
}
var JwtSecurityKey = Encoding.ASCII.GetBytes(jwtSecurityKeySection.Value);

var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true).Build();

builder.Services.AddControllers();
builder.Services.AddGrpc();
//Add Repositorys
builder.Services.AddScoped<GeneralAttributeRepository>();
//Add Services
builder.Services.AddScoped<SettingService>();
//Grpc
builder.Services.AddScoped<GameRoomProxy>();
builder.Services.AddScoped<AdapterHandler>();

builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("https://localhost:8080")
            .AllowCredentials();
    });
});
builder.Services.AddDbContext<MafiaDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging(false));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<MafiaDbContext>();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", Microsoft.Extensions.Logging.LogLevel.None);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireClaim(ClaimTypes.NameIdentifier);
    });
builder.Services.AddAuthentication(x =>
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

});

var grpcServer = config.GetSection("GrpcServerRoot").Value;
builder.Services.AddGrpcClient<GameRoomGrpc.GameRoomGrpcClient>("GameManaging", o =>
{
    if (grpcServer == null)
        throw new Exception("GrpcServerRoot is not configured in appsettings.json.");
    else
        o.Address = new Uri(uriString: grpcServer);
});


var app = builder.Build();
app.UseRouting();
app.UseCors("CorsPolicy");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<AdapterHandler>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapControllers(); // Add this line to map WebAPI controller endpoints

app.Run();
