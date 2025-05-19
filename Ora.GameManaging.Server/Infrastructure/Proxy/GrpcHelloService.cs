using Grpc.Net.ClientFactory;
using Ora.GameManaging.Mafia.Protos;
using System.Threading.Tasks;

namespace Ora.GameManaging.Server.Infrastructure.Proxy;
public class GrpcHelloService(GrpcClientFactory clientFactory)
{
    private readonly GrpcClientFactory _clientFactory = clientFactory;

    public async Task<string> SayHelloAsync(string gameType, string name)
    {
        var client = _clientFactory.CreateClient<Greeter.GreeterClient>(gameType);
        var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
        return reply.Message;
    }
}