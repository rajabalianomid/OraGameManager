namespace Ora.GameManaging.Server.Models.Adapter
{
    public class LastInformationRequestModel : AdapterModel
    {
        public override string ActionName => "PrepareLatestInformationAsync";
        public override string TypeName => "GameEngine";

        public required string RequestModel { get; set; }
    }
}
