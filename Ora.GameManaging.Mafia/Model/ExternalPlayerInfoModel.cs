namespace Ora.GameManaging.Mafia.Model
{
    public class ExternalPlayerInfoModel
    {
        public required string ACSToken { get; set; }
        public required DateTimeOffset ACSTokenExpire { get; set; }
    }
}
