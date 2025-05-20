namespace Ora.GameManaging.Mafia.Model
{
    public class LoginResultModel : BaseResultModel<string>
    {
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public string? ImageName { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
