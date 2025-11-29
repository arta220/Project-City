using Domain.Base;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Enums;
using Domain.Map;
using System.Collections.Generic;

namespace Domain.Buildings
{
    public abstract class CommercialBuilding : Building, IServiceBuilding
    {
        protected Queue<Citizen> visitorQueue = new Queue<Citizen>();
        protected Dictionary<Citizen, int> citizensInService = new Dictionary<Citizen, int>();

        public int ServiceTimeInTicks { get; protected set; }
        public int MaxQueueLength { get; protected set; }
        public int WorkerCount { get; protected set; }
        public abstract CommercialType CommercialType { get; }

        public int CurrentVisitors => citizensInService.Count;
        public bool CanAcceptMoreVisitors => CurrentVisitors < MaxOccupancy && visitorQueue.Count < MaxQueueLength;

        protected CommercialBuilding(Area area, int serviceTime, int maxQueue, int workerCount)
            : base(1, CalculateMaxOccupancy(workerCount), area)
        {
            ServiceTimeInTicks = serviceTime;
            MaxQueueLength = maxQueue;
            WorkerCount = workerCount;
        }

        private static int CalculateMaxOccupancy(int workerCount)
        {
            // Везде 3 посетителя на работника
            return workerCount * 3;
        }

        public virtual void EnqueueCitizen(Citizen citizen)
        {
            if (CanAcceptMoreVisitors)
            {
                visitorQueue.Enqueue(citizen);
            }
        }

        public virtual void Tick(int currentTick)
        {
            // Обработка завершения обслуживания
            var completedCitizens = new List<Citizen>();
            foreach (var kvp in citizensInService)
            {
                if (currentTick >= kvp.Value)
                {
                    completedCitizens.Add(kvp.Key);
                }
            }

            foreach (var citizen in completedCitizens)
            {
                citizensInService.Remove(citizen);
                citizen.State = CitizenState.Idle;
            }

            // Добавление новых граждан из очереди
            while (visitorQueue.Count > 0 && citizensInService.Count < MaxOccupancy)
            {
                var citizen = visitorQueue.Dequeue();
                int completionTick = currentTick + ServiceTimeInTicks;
                citizensInService[citizen] = completionTick;
            }
        }
    }
}