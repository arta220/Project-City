using Domain.Map;
using Domain.Citizens;
using Domain.Common.Time;

namespace Services.Citizens.Movement
{
    public interface ICitizenMovementService
    {
        void SetTarget(Citizen citizen, Position target);
        void PlayMovement(Citizen citizen, SimulationTime time);
        void RecalculatePath(Citizen citizen);
    }
}
