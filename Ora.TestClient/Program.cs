using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

enum PlayerStatus
{
    Online = 0,
    InGame = 1,
    Waiting = 2,
    Turn = 3
}

class Program
{
    static HubConnection connection = null!;
    static string? currentRoom = null;
    static string appId = "";
    static string playerName = "";
    static string userId = "";

    static async Task Main(string[] args)
    {
        args = ["--autojoin"]; // For testing purposes, remove this line in production
        if (args.Contains("--autojoin"))
        {
            Console.Write("AppId: ");
            var appId = Console.ReadLine() ?? "defaultApp";
            Console.Write("RoomId: ");
            var roomId = Console.ReadLine() ?? "TestRoom";
            Console.Write("Client count: ");
            var countStr = Console.ReadLine();
            int clientCount = int.TryParse(countStr, out var c) ? c : 5;

            var program = new Program();
            await program.TestAutoJoinClientsAsync(clientCount, appId, roomId);
            return;
        }

        Thread.Sleep(4000);

        Console.Write("Enter your AppId (client isolation): ");
        appId = Console.ReadLine() ?? "defaultApp";

        Console.Write("Enter your player name: ");
        playerName = Console.ReadLine() ?? "Player";

        userId = $"{appId}:{playerName}";

        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/gamehub")
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();

        await connection.StartAsync();
        Console.WriteLine("Connected to SignalR hub.\n");

        await connection.InvokeAsync("Reconnect", appId, userId);

        while (true)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Create room");
            Console.WriteLine("2. Delete room");
            Console.WriteLine("3. List rooms");
            Console.WriteLine("4. Join room");
            Console.WriteLine("5. Leave room");
            Console.WriteLine("6. List players in current room");
            Console.WriteLine("7. Update my status");
            Console.WriteLine("8. Send message to room");
            Console.WriteLine("9. Start group turn (by role or connectionId)");
            Console.WriteLine("10. Pause group timer");
            Console.WriteLine("11. Resume group timer");
            Console.WriteLine("12. Start rotating group turn (single-pass)");
            Console.WriteLine("13. Change player role");
            Console.WriteLine("14. Exit");
            Console.Write("Select option: ");

            var input = Console.ReadLine();
            try
            {
                switch (input)
                {
                    case "1":
                        Console.Write("Enter room ID to create: ");
                        var createId = Console.ReadLine();
                        await connection.InvokeAsync("CreateRoom", appId, createId);
                        break;
                    case "2":
                        Console.Write("Enter room ID to delete: ");
                        var deleteId = Console.ReadLine();
                        await connection.InvokeAsync("DeleteRoom", appId, deleteId);
                        break;
                    case "3":
                        await connection.InvokeAsync("ListRooms", appId);
                        break;
                    case "4":
                        Console.Write("Enter room ID to join: ");
                        var joinId = Console.ReadLine();
                        Console.Write("Enter your role (e.g. Mafia, Villager, Doctor, etc): ");
                        var role = Console.ReadLine() ?? "";
                        currentRoom = joinId;
                        await connection.InvokeAsync("JoinRoom", appId, joinId, userId, playerName, role);
                        break;
                    case "5":
                        if (currentRoom != null)
                        {
                            await connection.InvokeAsync("LeaveRoom", appId, currentRoom, userId);
                            currentRoom = null;
                        }
                        else Console.WriteLine("You are not in a room.");
                        break;
                    case "6":
                        if (currentRoom != null)
                            await connection.InvokeAsync("GetPlayersInRoom", appId, currentRoom);
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "7":
                        if (currentRoom != null)
                        {
                            Console.WriteLine("Select status: 0=Online, 1=InGame, 2=Waiting, 3=Turn");
                            var status = Console.ReadLine();
                            if (Enum.TryParse<PlayerStatus>(status, out var result))
                                await connection.InvokeAsync("UpdateStatus", appId, currentRoom, userId, result);
                            else Console.WriteLine("Invalid status.");
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "8":
                        if (currentRoom != null)
                        {
                            Console.Write("Message: ");
                            var msg = Console.ReadLine();
                            await connection.InvokeAsync("SendMessageToRoom", appId, currentRoom, userId, msg);
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "9":
                        if (currentRoom != null)
                        {
                            Console.Write("Enter role(s) or connectionId(s) for group turn (comma-separated, leave empty for all): ");
                            var rolesOrIds = Console.ReadLine();
                            await connection.InvokeAsync("StartGroupTurn", appId, currentRoom, rolesOrIds);
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "10":
                        if (currentRoom != null)
                            await connection.InvokeAsync("PauseGroupTimer", appId, currentRoom);
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "11":
                        if (currentRoom != null)
                            await connection.InvokeAsync("ResumeGroupTimer", appId, currentRoom);
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "12":
                        if (currentRoom != null)
                        {
                            Console.Write("Enter role(s) or connectionId(s) for rotating group turn (comma-separated, leave empty for all): ");
                            var rolesOrIds = Console.ReadLine();
                            await connection.InvokeAsync("StartGroupTurnRotating", appId, currentRoom, rolesOrIds);
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "13":
                        if (currentRoom != null)
                        {
                            Console.Write("Enter new role: ");
                            var newRole = Console.ReadLine();
                            await connection.InvokeAsync("ChangePlayerRole", appId, currentRoom, userId, newRole);
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "14":
                        await connection.StopAsync();
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Exception] {ex.Message}");
            }
        }
    }

    static void RegisterHandlers()
    {
        connection.Reconnected += async (connectionId) =>
        {
            Console.WriteLine("[Reconnected] Try to auto-join...");
            await connection.InvokeAsync("Reconnect", appId, userId);
        };

        connection.Closed += async (error) =>
        {
            Console.WriteLine("Disconnected from server. Retrying...");
            while (true)
            {
                try
                {
                    await Task.Delay(2000);
                    await connection.StartAsync();
                    Console.WriteLine("Reconnected!");
                    break;
                }
                catch
                {
                    Console.WriteLine("Retrying connection...");
                }
            }
        };

        connection.On<string>("RoomCreated", roomId =>
        {
            Console.WriteLine($"Room created: {roomId}");
        });

        connection.On<string>("RoomDeleted", roomId =>
        {
            Console.WriteLine($"Room deleted: {roomId}");
        });

        connection.On<List<string>>("ReceiveRoomList", rooms =>
        {
            Console.WriteLine("Rooms:");
            foreach (var r in rooms)
                Console.WriteLine(" - " + r);
        });

        connection.On<string>("PlayerJoined", name =>
        {
            Console.WriteLine($"Player joined: {name}");
        });

        connection.On<string>("PlayerLeft", name =>
        {
            Console.WriteLine($"Player left: {name}");
        });

        connection.On<List<object>>("ReceivePlayerList", players =>
        {
            Console.WriteLine("Players:");
            foreach (var p in players)
            {
                try
                {
                    var json = (JsonElement)p;
                    var name = json.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "?";
                    var status = json.TryGetProperty("status", out var statusProp) ? statusProp.GetString() : "?";
                    var role = json.TryGetProperty("role", out var roleProp) ? roleProp.GetString() : "?";
                    Console.WriteLine($" - {name} ({status}) | Role: {role}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error parsing player] {ex.Message}");
                }
            }
        });

        connection.On<string, string>("PlayerStatusUpdated", (name, status) =>
        {
            Console.WriteLine($"{name} changed status to {status}");
        });

        connection.On<string, string>("ReceiveMessage", (name, message) =>
        {
            Console.WriteLine($"{name}: {message}");
        });

        connection.On<string>("Error", errorMsg =>
        {
            Console.WriteLine($"[Error] {errorMsg}");
        });

        connection.On<List<string>>("ReconnectedRooms", rooms =>
        {
            if (rooms.Count > 0)
            {
                Console.WriteLine("Auto-joined rooms after reconnect:");
                foreach (var room in rooms)
                    Console.WriteLine(" - " + room);
                currentRoom = rooms[0];
            }
        });

        // Optional: if your server supports these events, register them here.
        connection.On<string>("TurnChanged", msg =>
        {
            Console.WriteLine($"--- Turn changed! {msg} ---");
        });
        connection.On<int>("TimerTick", seconds =>
        {
            Console.WriteLine($"[TIMER] {seconds} seconds left");
        });
        connection.On<object>("TurnInfo", info =>
        {
            Console.WriteLine("[TurnInfo] Received: " + JsonSerializer.Serialize(info));
        });
        connection.On<string>("TurnTimeout", msg =>
        {
            Console.WriteLine($"!!! Turn timeout: {msg}");
        });
        connection.On("TimerPaused", () =>
        {
            Console.WriteLine("=== TIMER PAUSED ===");
        });
        connection.On("TimerResumed", () =>
        {
            Console.WriteLine("=== TIMER RESUMED ===");
        });

        connection.On<string, string>("PlayerRoleChanged", (name, newRole) =>
        {
            Console.WriteLine($"{name} changed role to {newRole}");
        });
    }
    async Task TestAutoJoinClientsAsync(int clientCount, string appId, string roomId)
    {
        Thread.Sleep(4000);
        var clients = new List<HubConnection>();

        for (int i = 0; i < clientCount; i++)
        {
            Thread.Sleep(4000);
            var playerName = $"TestUser_{i + 1}";
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/gamehub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<int>("TimerTick", seconds =>
            {
                Console.WriteLine($"[{playerName}] [TIMER] {seconds} seconds left");
            });
            connection.On<object>("TurnInfo", info =>
            {
                Console.WriteLine("[TurnInfo] Received: " + JsonSerializer.Serialize(info));
            });

            await connection.StartAsync();
            clients.Add(connection);

            await connection.InvokeAsync("JoinRoomAuto", appId, roomId, playerName);

            Console.WriteLine($"[{playerName}] joined room {roomId}");
        }

        Console.WriteLine("All test clients joined. Press any key to disconnect...");
        Console.ReadKey();

        foreach (var client in clients)
        {
            await client.StopAsync();
            await client.DisposeAsync();
        }
    }
}
