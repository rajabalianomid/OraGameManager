﻿using System.Text.Json;

namespace Ora.GameManaging.Server.Infrastructure
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLowerInvariant();
    }
}
