namespace Ora.GameManaging.Server.Models.Adapter;
public class RemoveRoleModel : AdapterModel
{
    public override string ActionName => "RemoveAssignedRoleAsync";
    public override string TypeName => "SettingService";

    public string ApplicationInstanceId { get; set; } = "";
    public string RoomId { get; set; } = "";
    public string UserId { get; set; } = "";
}