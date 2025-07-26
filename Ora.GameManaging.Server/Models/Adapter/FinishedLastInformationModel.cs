namespace Ora.GameManaging.Server.Models.Adapter
{
    public class ConfirmLastInformationRequestModel : AdapterModel
    {
        public override string ActionName => "ConfirmLatestInformationAsync";
        public override string TypeName => "GameEngine";

        public required string RequestModel { get; set; }
    }
}
