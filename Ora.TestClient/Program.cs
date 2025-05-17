using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

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
    static string playerName = "";

    static async Task Main()
    {
        Console.Write("Enter your player name: ");
        playerName = Console.ReadLine() ?? "Player";

        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/gamehub")
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();
        await connection.StartAsync();
        Console.WriteLine("Connected to SignalR hub.\n");

        while (true)
        {
            Console.WriteLine("--- MENU ---");
            Console.WriteLine("1. Create room");
            Console.WriteLine("2. Delete room");
            Console.WriteLine("3. List rooms");
            Console.WriteLine("4. Join room");
            Console.WriteLine("5. Leave room");
            Console.WriteLine("6. List players in current room");
            Console.WriteLine("7. Update my status");
            Console.WriteLine("8. Send message to room");
            Console.WriteLine("9. Start turn cycle");
            Console.WriteLine("10. Exit");
            Console.Write("Select option: ");

            var input = Console.ReadLine();
            try
            {
                switch (input)
                {
                    case "1":
                        Console.Write("Enter room ID to create: ");
                        var createId = Console.ReadLine();
                        await connection.InvokeAsync("CreateRoom", createId);
                        break;
                    case "2":
                        Console.Write("Enter room ID to delete: ");
                        var deleteId = Console.ReadLine();
                        await connection.InvokeAsync("DeleteRoom", deleteId);
                        break;
                    case "3":
                        await connection.InvokeAsync("ListRooms");
                        break;
                    case "4":
                        Console.Write("Enter room ID to join: ");
                        var joinId = Console.ReadLine();
                        currentRoom = joinId;
                        await connection.InvokeAsync("JoinRoom", joinId, playerName);
                        break;
                    case "5":
                        if (currentRoom != null)
                        {
                            await connection.InvokeAsync("LeaveRoom", currentRoom);
                            currentRoom = null;
                        }
                        else Console.WriteLine("You are not in a room.");
                        break;
                    case "6":
                        if (currentRoom != null)
                            await connection.InvokeAsync("GetPlayersInRoom", currentRoom);
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "7":
                        if (currentRoom != null)
                        {
                            Console.WriteLine("Select status: 0=Online, 1=InGame, 2=Waiting, 3=Turn");
                            var status = Console.ReadLine();
                            if (Enum.TryParse<PlayerStatus>(status, out var result))
                                await connection.InvokeAsync("UpdateStatus", currentRoom, result);
                            else Console.WriteLine("Invalid status.");
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "8":
                        if (currentRoom != null)
                        {
                            Console.Write("Message: ");
                            var msg = Console.ReadLine();
                            await connection.InvokeAsync("SendMessageToRoom", currentRoom, msg);
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "9":
                        if (currentRoom != null)
                        {
                            await connection.InvokeAsync("StartTurnCycle", currentRoom);
                        }
                        else Console.WriteLine("Join a room first.");
                        break;
                    case "10":
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

            Console.WriteLine();
        }
    }

    static void RegisterHandlers()
    {
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
            rooms.ForEach(r => Console.WriteLine(" - " + r));
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
                    Console.WriteLine($" - {name} ({status})");
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

        connection.On<string>("TurnChanged", playerName =>
        {
            Console.WriteLine($"🔄 Turn changed! It's now {playerName}'s turn.");
        });

        connection.On<int>("TimerTick", remaining =>
        {
            Console.WriteLine($"⏱ Time remaining: {remaining}s");
        });

        connection.On<string>("TurnTimeout", playerName =>
        {
            Console.WriteLine($"⏰ {playerName}'s turn has timed out!");
        });
    }
}
