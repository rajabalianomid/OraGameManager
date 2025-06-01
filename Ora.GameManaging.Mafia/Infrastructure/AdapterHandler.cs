using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ora.GameManaging.Mafia.Protos;
using System.Reflection;
using System.Text.Json;

namespace Ora.GameManaging.Mafia.Infrastructure
{
    public class AdapterHandler(IServiceProvider serviceProvider) : AdapterGrpc.AdapterGrpcBase
    {
        public override async Task<AdapterReply> Run(AdapterRequest request, ServerCallContext context)
        {
            var typeName = request.TypeName;
            var actionName = request.Action;

            // Find the service class by typeName
            var type = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => t.Name == typeName);

            if (type == null)
                return new AdapterReply { Error = $"Type '{typeName}' not found." };

            // Resolve the service instance from the DI container
            var instance = serviceProvider.GetService(type);
            if (instance == null)
                return new AdapterReply { Error = $"Service '{typeName}' not registered in DI container." };

            // Find the method by action name
            var method = type.GetMethod(actionName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
                return new AdapterReply { Error = $"Action '{actionName}' not found in type '{typeName}'." };

            var parameters = method.GetParameters();
            object?[] invokeParams;

            if (parameters.Length > 0)
            {
                if (string.IsNullOrWhiteSpace(request.ModelJson))
                    return new AdapterReply { Error = "ModelJson is required for this action." };

                var paramValues = new object?[parameters.Length];
                using var doc = JsonDocument.Parse(request.ModelJson);

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    JsonElement? foundElement = null;

                    // Enumerate all properties and do a case-insensitive comparison
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (string.Equals(prop.Name, param.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            foundElement = prop.Value;
                            break;
                        }
                    }

                    if (foundElement != null)
                    {
                        paramValues[i] = foundElement.Value.Deserialize(param.ParameterType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        paramValues[i] = param.HasDefaultValue ? param.DefaultValue : GetDefault(param.ParameterType);
                    }
                }

                invokeParams = paramValues;
            }
            else
            {
                invokeParams = Array.Empty<object>();
            }

            object? result;

            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                // Invoke async method
                var taskObj = method.Invoke(instance, invokeParams);
                if (taskObj == null)
                    return new AdapterReply { Error = "Async method invocation failed." };

                var task = (Task)taskObj;
                await task.ConfigureAwait(false);

                // If method returns Task<T>, get Result property
                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultProperty = task.GetType().GetProperty("Result");
                    result = resultProperty?.GetValue(task);
                }
                else
                {
                    result = null;
                }
            }
            else
            {
                // Invoke sync method
                result = await Task.Run(() => method.Invoke(instance, invokeParams));
            }

            // Serialize the result to JSON
            string? resultJson = result != null ? JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }) : null;
            return new AdapterReply { DataJson = resultJson };
        }

        static object? GetDefault(System.Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}
