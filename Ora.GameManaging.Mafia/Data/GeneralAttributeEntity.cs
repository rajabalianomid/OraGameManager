namespace Ora.GameManaging.Mafia.Data
{
    public class GeneralAttributeEntity
    {
        public int Id { get; set; }
        public required string ApplicationInstanceId { get; set; }
        public required string EntityName { get; set; }
        public required string EntityId { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}