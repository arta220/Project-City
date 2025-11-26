using Domain.Base;
using Domain.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class CommercialBuilding : Building
    {
        public CommercialType Type { get; }
        public int RequiredWorkers { get; }
        public int MaxVisitors { get; }
        public int CurrentVisitors { get; set; }

        public CommercialBuilding(int floors, int maxOccupancy, Area area,
                                CommercialType type, int requiredWorkers, int maxVisitors)
            : base(floors, maxOccupancy, area)
        {
            Type = type;
            RequiredWorkers = requiredWorkers;
            MaxVisitors = maxVisitors;
            CurrentVisitors = 0;
        }

        public override CommercialBuilding Clone()
        {
            return new CommercialBuilding(Floors, MaxOccupancy,
                new Area(Area.Width, Area.Height), Type, RequiredWorkers, MaxVisitors);
        }
    }
}