using Domain.Buildings;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings.Utility
{
    /// <summary>
    /// Офис ЖКХ - база для работников коммунальных служб
    /// </summary>
    public class UtilityOffice : Building, IServiceBuilding
    {
        private int _currentVisitors = 0;
        private int _currentQueue = 0;

        public UtilityOffice(Area area)
            : base(floors: 1, maxOccupancy: 10, area)
        {
        }

        public bool TryEnter()
        {
            if (_currentVisitors < MaxOccupancy)
            {
                _currentVisitors++;
                return true;
            }
            return false;
        }

        public void Leave()
        {
            if (_currentVisitors > 0)
                _currentVisitors--;
        }

        public bool TryJoinQueue()
        {
            if (_currentQueue < MaxQueueLength)
            {
                _currentQueue++;
                return true;
            }
            return false;
        }

        public void LeaveQueue()
        {
            if (_currentQueue > 0)
                _currentQueue--;
        }

        public void ProcessQueue()
        {
            // Для офиса ЖКХ не нужна обработка очереди как в коммерческих зданиях
            // Работники просто находятся здесь между заданиями
        }

        public int CurrentVisitors => _currentVisitors;
        public int CurrentQueue => _currentQueue;
        public bool CanAcceptMoreVisitors => _currentVisitors < MaxOccupancy;
        public CommercialType CommercialType => CommercialType.Service;
        public int ServiceTimeInTicks => 0; // Не используется
        public int MaxQueueLength => 20;
    }
}