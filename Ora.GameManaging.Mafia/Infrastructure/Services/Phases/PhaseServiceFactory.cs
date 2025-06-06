namespace Ora.GameManaging.Mafia.Infrastructure.Services.Phases
{
    public class PhaseServiceFactory(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public BasePhaseService GetPhaseService(string phase)
        {
            // Normalize phase name to match class names
            var typeName = $"{phase}PhaseService";
            var phaseServiceType = Type.GetType($"Ora.GameManaging.Mafia.Infrastructure.Services.Phases.{typeName}");

            if (phaseServiceType != null && typeof(BasePhaseService).IsAssignableFrom(phaseServiceType))
            {
                var service = _serviceProvider.GetService(phaseServiceType) as BasePhaseService;
                if (service != null)
                    return service;
            }

            // Fallback to base phase service
            return _serviceProvider.GetRequiredService<BasePhaseService>();
        }
    }
}