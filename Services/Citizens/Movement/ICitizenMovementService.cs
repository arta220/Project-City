using Domain.Map;
using Domain.Citizens;
using Domain.Common.Time;

namespace Services.Citizens.Movement
{
    public interface ICitizenMovementService
    {
        void Move(Citizen citizen, Position position, SimulationTime time);
    }
}
