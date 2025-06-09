using System.Text.Json.Serialization;

namespace Ora.GameManaging.Server.Models
{
    public class ExtraInfoDetails
    {
        public List<string> ForceNextTurns { get; set; } = [];
        public List<dynamic> NotInCurrentTurn { get; set; } = [];
    }
}
