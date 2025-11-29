using Domain.Map;
using Domain.Citizens;
using Domain.Time;

namespace Services.Interfaces
{
    public interface ICitizenMovementService
    {
        void Move(Citizen citizen, Position position, SimulationTime time);
    }
}
