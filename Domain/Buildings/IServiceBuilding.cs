using Domain.Citizens;
using Domain.Map;
using Domain.Enums;

namespace Domain.Buildings
{
    public interface IServiceBuilding
    {
        void EnqueueCitizen(Citizen citizen);
        void Tick(int currentTick);
        int CurrentVisitors { get; }
        bool CanAcceptMoreVisitors { get; }
        CommercialType CommercialType { get; }
    }
}