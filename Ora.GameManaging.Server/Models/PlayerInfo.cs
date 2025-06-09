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
    }
}
