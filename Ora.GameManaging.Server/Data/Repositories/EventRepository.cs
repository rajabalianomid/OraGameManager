namespace Ora.GameManaging.Server.Data.Repositories
{
    public class EventRepository
    {
        private readonly GameDbContext _db;
        public EventRepository(GameDbContext db) => _db = db;

        public async Task AddAsync(int roomId, string eventType, string? playerName, string? data)
        {
            var evt = new GameEventEntity
            {
                GameRoomEntityId = roomId,
                PlayerName = playerName,
                EventType = eventType,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
            _db.Events.Add(evt);
            await _db.SaveChangesAsync();
        }
    }
}
