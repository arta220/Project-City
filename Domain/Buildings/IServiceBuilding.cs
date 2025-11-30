using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public interface IServiceBuilding
    {
        bool TryEnter();
        void Leave();
        bool TryJoinQueue();
        void LeaveQueue();
        void ProcessQueue();
        int CurrentVisitors { get; }
        int CurrentQueue { get; }
        bool CanAcceptMoreVisitors { get; }
        CommercialType CommercialType { get; }
        int ServiceTimeInTicks { get; }
        int MaxOccupancy { get; }
        int MaxQueueLength { get; }
    }
}