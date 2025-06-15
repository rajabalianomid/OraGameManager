using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ora.GameManaging.Server.Models
{
    public class PlayerInfo : BaseUserModel
    {
        public string ConnectionId { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Role { get; set; }
        public PlayerStatus Status { get; set; } = PlayerStatus.Online;
        public List<ExtraPlayetInfo> ExtraInfos { get; set; } = [];

    }
    public class ExtraPlayetInfo
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}
