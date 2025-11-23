using Domain.Map;
using Domain.Citizens;

namespace Services.Interfaces
{
    public interface ICitizenMovementService
    {
        void Move(Citizen citizen, Position position);
    }
}
