namespace Ora.GameManaging.Mafia.Model
{
    public enum PhaseStatus
    {
        Lobby = 1,
        Talk = 2,
        Vote = 3,
        Defense = 4,
        FinalVote = 5,
        CardLastChance = 6,
        CardLastChanceReport = 7,
        MafiaTalk = 8,
        Action = 9,
    }
    public enum AbilityTypes
    {
        Report = 1,
    }

    public static class EnumsHelper
    {
        public static string GetNextPhaseName(this string phase)
        {
            // Convert phase string to enum
            if (!Enum.TryParse<PhaseStatus>(phase, out var phaseEnum))
            {
                // Handle invalid phase string, e.g., default to Lobby or throw
                phaseEnum = PhaseStatus.Lobby;
            }

            // Get next phase
            int next = ((int)phaseEnum + 1) % Enum.GetValues<PhaseStatus>().Length;
            var nextPhase = ((PhaseStatus)next).ToString();
            return nextPhase;
        }
        public static string GetPreviousPhaseName(this string phase)
        {
            if (!Enum.TryParse<PhaseStatus>(phase, out var currentPhase))
            {
                currentPhase = PhaseStatus.Lobby;
            }

            var values = Enum.GetValues<PhaseStatus>().OrderBy(p => (int)p).ToList();
            int index = values.IndexOf(currentPhase);

            if (index <= 0)
                return values.Last().ToString(); // اگه اول لیست بود، برگرد آخرین فاز

            return values[index - 1].ToString();
        }

    }
}
