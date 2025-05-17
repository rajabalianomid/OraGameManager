using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ora.GameManaging.Server.Models
{
    public class PlayerInfo
    {
        public string ConnectionId { get; set; } = "";
        public string Name { get; set; } = "";
        public PlayerStatus Status { get; set; } = PlayerStatus.Online;
    }
}
