namespace Ora.GameManaging.Server.Models.Adapter
{
    public class LastInformationModel : AdapterModel
    {
        public override string ActionName => "PrepareLatestInformationAsync";
        public override string TypeName => "MafiaEngine";

        public required string RequestModel { get; set; }
    }
}
