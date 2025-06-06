using Microsoft.Extensions.DependencyInjection;
using Ora.GameManaging.Server.Data.Repositories;
using Ora.GameManaging.Server.Infrastructure.Proxy;
using Ora.GameManaging.Server.Models;
using Ora.GameManaging.Server.Models.Adapter;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ora.GameManaging.Server.Infrastructure
{
    /// <summary>
    /// Handles all game lifecycle logic, including game start, player management, and turn management.
    /// </summary>
    public class GameManager
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly GameRoomRepository _roomRepo;
        //private readonly PlayerRepository _playerRepo;
        //private readonly EventRepository _eventRepo;
        private readonly NotificationManager _notification;
        private readonly TurnManager _turnManager;
        private readonly GrpcAdapter _grpcAdapter;

        // Key: AppId:RoomId
        internal static ConcurrentDictionary<string, GameRoom> Rooms = new();

        // Key: AppId:RoomId -> CancellationTokenSource for each room's game loop
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _gameLoopTokens = new();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _turnCompletionSources = new();

        public GameManager(
        IServiceProvider serviceProvider,
        //GameRoomRepository roomRepo,
        //PlayerRepository playerRepo,
        //EventRepository eventRepo,
        NotificationManager notification,
        TurnManager turnManager,
        GrpcAdapter grpcAdapter)
        {
            //_roomRepo = roomRepo;
            //_playerRepo = playerRepo;
            //_eventRepo = eventRepo;
            _notification = notification;
            _turnManager = turnManager;
            _grpcAdapter = grpcAdapter;
            _serviceProvider = serviceProvider;
            turnManager.RegisterTurnFinishedCallback(NotifyTurnFinished);
        }

        /// <summary>
        /// Call this after loading all rooms from the database to resume game loops for rooms with active games.
        /// </summary>
        public async Task ResumeActiveGamesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var roomRepo = scope.ServiceProvider.GetRequiredService<GameRoomRepository>();
            foreach (var kvp in Rooms)
            {
                var key = kvp.Key;
                var room = kvp.Value;
                var dbRoom = await roomRepo.GetByRoomIdAsync(room.AppId, room.RoomId);
                if (dbRoom != null && dbRoom.IsGameStarted && !IsGameOver(room))
                {
                    StartGameLoop(room.AppId, room.RoomId, room, skipFirstPhaseChange: true);
                }
            }
        }

        /// <summary>
        /// Should be called after a player joins a room. Checks if the game should start.
        /// </summary>
        public async Task OnPlayerJoinedAsync(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (!Rooms.TryGetValue(key, out var room))
                return;

            // Retrieve MaxPlayer from the database or cache
            int maxPlayer = await _grpcAdapter.Do<int, MaxRoomPlayerModel>(new MaxRoomPlayerModel { ApplicationInstanceId = appId, RoomId = roomId });

            if (room.Players.Count == maxPlayer)
            {
                // Start the game if the required number of players is reached
                await StartGameAsync(appId, roomId, room);
            }
        }

        /// <summary>
        /// Starts the game, triggers the first rotating group turn, and launches the game loop.
        /// </summary>
        private async Task StartGameAsync(string appId, string roomId, GameRoom room)
        {
            using var scope = _serviceProvider.CreateScope();
            var roomRepo = scope.ServiceProvider.GetRequiredService<GameRoomRepository>();
            var eventRepo = scope.ServiceProvider.GetRequiredService<EventRepository>();

            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom == null) return;

            dbRoom.IsGameStarted = true;
            await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());
            await eventRepo.AddAsync(dbRoom.Id, "GameStarted", null, null);

            await _notification.SendTurnChangedToPlayers(
                room.Players.Values.Select(p => p.ConnectionId).ToList(),
                room,
                "Game started! First turn."
            );

            // Start the game loop for this room
            StartGameLoop(appId, roomId, room, skipFirstPhaseChange: true);
        }

        /// <summary>
        /// Main game loop for a room. Runs until the game is over or cancelled.
        /// </summary>
        private void StartGameLoop(string appId, string roomId, GameRoom room, bool skipFirstPhaseChange = false)
        {
            var key = $"{appId}:{roomId}";
            // Prevent duplicate loops for the same room
            if (_gameLoopTokens.ContainsKey(key))
                return;

            var cts = new CancellationTokenSource();
            _gameLoopTokens[key] = cts;

            _ = Task.Run(async () =>
            {
                try
                {
                    bool isFirstLoop = true;
                    while (!cts.Token.IsCancellationRequested)
                    {
                        // Check if the game is over
                        if (IsGameOver(room))
                        {
                            await OnGameOverAsync(appId, roomId, room);
                            break;
                        }

                        // Build the rotating queue at the start of each round
                        var rotatingQueue = await BuildCustomPlayerQueue(room);

                        // Start rotating turn for this queue
                        _turnManager.StartGroupTurnRotating(key, rotatingQueue, room.TurnDurationSeconds);

                        if (!(skipFirstPhaseChange && isFirstLoop))
                        {
                            await GoToNextGamePhase(room);
                        }

                        isFirstLoop = false;

                        // Wait for the turn to finish (implement a mechanism or event to know when to continue)
                        await WaitForTurnToFinishAsync(key, cts.Token);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Game loop was cancelled (room deleted or server shutdown)
                }
                finally
                {
                    _gameLoopTokens.TryRemove(key, out _);
                }
            }, cts.Token);
        }

        // Example: Build your custom queue based on your game logic
        private async Task<List<object>> BuildCustomPlayerQueue(GameRoom room)
        {
            var result = await _grpcAdapter.Do<object, TurnModel>(new TurnModel { ApplicationInstanceId = room.AppId, RoomId = room.RoomId });

            // Handle both JsonElement and JsonArray (for flexibility)
            if (result is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
            {
                var queue = new List<object>();
                foreach (var item in jsonElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        queue.Add(item.GetString() ?? string.Empty);
                    }
                    else if (item.ValueKind == JsonValueKind.Array)
                    {
                        var group = item.EnumerateArray()
                                        .Where(e => e.ValueKind == JsonValueKind.String)
                                        .Select(e => e.GetString() ?? string.Empty)
                                        .ToList();
                        if (group.Count == 1)
                            queue.Add(group[0]);
                        else if (group.Count > 1)
                            queue.Add(group);
                    }
                }
                return queue;
            }
            return [];
        }

        // Example: Wait for the turn to finish (implement this based on your timer/turn logic)
        private async Task WaitForTurnToFinishAsync(string key, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _turnCompletionSources[key] = tcs;
            using (token.Register(() => tcs.TrySetCanceled()))
            {
                await tcs.Task; // Wait until notified
            }
            _turnCompletionSources.TryRemove(key, out _);
        }

        // This method should be called by TurnManager when the rotating turn is finished
        public void NotifyTurnFinished(string key)
        {
            if (_turnCompletionSources.TryGetValue(key, out var tcs))
                tcs.TrySetResult(true);
        }

        /// <summary>
        /// Determines if the game is over. Adjust this logic to fit your game rules.
        /// </summary>
        private bool IsGameOver(GameRoom room)
        {
            // Example: Game is over if only one player is online
            var alivePlayers = room.Players.Values.Count(p => p.Status == PlayerStatus.Online);
            return alivePlayers <= 1;
        }

        /// <summary>
        /// Called when the game is over. Handles cleanup and notifications.
        /// </summary>
        private async Task OnGameOverAsync(string appId, string roomId, GameRoom room)
        {
            using var scope = _serviceProvider.CreateScope();
            var roomRepo = scope.ServiceProvider.GetRequiredService<GameRoomRepository>();
            var eventRepo = scope.ServiceProvider.GetRequiredService<EventRepository>();

            var dbRoom = await roomRepo.GetByRoomIdAsync(appId, roomId);
            if (dbRoom != null)
            {
                dbRoom.IsGameStarted = false;
                await roomRepo.SaveSnapshotAsync(appId, roomId, room.Serialize());
                await eventRepo.AddAsync(dbRoom.Id, "GameOver", null, null);
            }

            var playerIds = room.Players.Values.Select(p => p.ConnectionId).ToList();
            await _notification.SendTurnChangedToPlayers(playerIds, room, "Game over!");

            // Add any additional cleanup logic here if needed
        }

        /// <summary>
        /// Stops the game loop for a room (e.g., when the room is deleted).
        /// </summary>
        public void StopGameLoop(string appId, string roomId)
        {
            var key = $"{appId}:{roomId}";
            if (_gameLoopTokens.TryRemove(key, out var cts))
                cts.Cancel();
        }

        public async Task<string> GoToNextGamePhase(GameRoom room)
        {
            var result = await _grpcAdapter.Do<NextPhaseResponseModel, NextPhaseModel>(new NextPhaseModel { currentPhase = room.Phase });
            using var scope = _serviceProvider.CreateScope();
            var roomRepo = scope.ServiceProvider.GetRequiredService<GameRoomRepository>();
            var foundRoom = await roomRepo.UpdatePhaseAsync(room.AppId, room.RoomId, result.Name, result.IsLastPhase);
            room.CurrentTurnPlayerId = foundRoom.CurrentTurnPlayer;
            room.Phase = foundRoom.Phase;
            room.Round = foundRoom.Round;
            return result.Name;
        }
    }
}