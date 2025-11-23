using CitySkylines_REMAKE.Models.Citizens;
using CitySkylines_REMAKE.Models.Map;

namespace CitySkylines_REMAKE.Services.Interfaces
{
    public interface ICitizenMovementService
    {
        void Move(Citizen citizen, Position position);
    }
}
