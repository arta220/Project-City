using Domain.Common.Base;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public abstract class CommercialBuilding : Building, IServiceBuilding
    {
        public int ServiceTimeInTicks { get; protected set; }
        public int MaxQueueLength { get; protected set; }
        public int WorkerCount { get; protected set; }
        public abstract CommercialType CommercialType { get; }

        public int CurrentVisitors { get; private set; }
        public int CurrentQueue { get; private set; }

        public bool CanAcceptMoreVisitors => CurrentVisitors < MaxOccupancy && CurrentQueue < MaxQueueLength;

        protected CommercialBuilding(Area area, int serviceTime, int maxQueue, int workerCount)
            : base(1, CalculateMaxOccupancy(workerCount), area)
        {
            ServiceTimeInTicks = serviceTime;
            MaxQueueLength = maxQueue;
            WorkerCount = workerCount;
        }

        private static int CalculateMaxOccupancy(int workerCount)
        {
            return workerCount * 3;
        }

        public bool TryEnter()
        {
            if (CurrentVisitors < MaxOccupancy)
            {
                CurrentVisitors++;
                return true;
            }
            return false;
        }

        public void Leave()
        {
            if (CurrentVisitors > 0)
                CurrentVisitors--;
        }

        public bool TryJoinQueue()
        {
            if (CurrentQueue < MaxQueueLength)
            {
                CurrentQueue++;
                return true;
            }
            return false;
        }

        public void LeaveQueue()
        {
            if (CurrentQueue > 0)
                CurrentQueue--;
        }

        public void ProcessQueue()
        {
            while (CurrentQueue > 0 && CurrentVisitors < MaxOccupancy)
            {
                CurrentQueue--;
                CurrentVisitors++;
            }
        }
    }
}