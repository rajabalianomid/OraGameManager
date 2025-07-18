using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Model
{
    public class AbilityModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Expression { get; set; }
        public bool IsNightAbility { get; set; }
        public bool IsDayAbility { get; set; }
        public string? RelatedPhase { get; set; }
        public string? Icon { get; set; }
        public bool IsCard { get; set; }

        public AbilityModel() { }

        public AbilityModel(AbilityEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            Expression = entity.Expression;
            IsNightAbility = entity.IsNightAbility;
            IsDayAbility = entity.IsDayAbility;
            RelatedPhase = entity.RelatedPhase;
            Icon = entity.Icon;
            IsCard = entity.IsCard;
        }
    }
}