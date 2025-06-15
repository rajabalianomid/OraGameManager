namespace Ora.GameManaging.Server.Data
{
    public class GeneralAttributeEntity
    {
        public int Id { get; set; }
        public string Key { get; set; } = "";
        public string? Value { get; set; }
        public required string EntityKey { get; set; }
        public required string EntityId { get; set; }
        public string? Description { get; set; }
    }
}