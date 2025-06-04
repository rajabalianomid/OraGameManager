using Grpc.Net.ClientFactory;
using Ora.GameManaging.Mafia.Protos;
using Ora.GameManaging.Server.Models.Adapter;
using System.Text.Json;
using static Ora.GameManaging.Mafia.Protos.AdapterGrpc;

namespace Ora.GameManaging.Server.Infrastructure.Proxy;
public class GrpcAdapter(GrpcClientFactory clientFactory)
{
    const string clientName = "Mafia_Adapter";

    public async Task<TOut> Do<TOut, TIn>(TIn model) where TIn : AdapterModel
    {
        var client = clientFactory.CreateClient<AdapterGrpcClient>(clientName) ?? throw new InvalidOperationException($"Failed to create gRPC client with name '{clientName}'.");
        var response = await client.RunAsync(new AdapterRequest
        {
            Action = model.ActionName,
            ModelJson = JsonSerializer.Serialize(model),
            TypeName = model.TypeName
        });

        // Assuming TOut is deserialized from the response. Adjust as needed.
        return JsonSerializer.Deserialize<TOut>(response.DataJson ?? string.Empty, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
               ?? throw new InvalidOperationException("Failed to deserialize the response.");
    }
    public async Task Do<TIn>(TIn model) where TIn : AdapterModel
    {
        var client = clientFactory.CreateClient<AdapterGrpcClient>(clientName) ?? throw new InvalidOperationException($"Failed to create gRPC client with name '{clientName}'.");
        _ = await client.RunAsync(new AdapterRequest
        {
            Action = model.ActionName,
            ModelJson = JsonSerializer.Serialize(model),
            TypeName = model.TypeName
        });
    }
}