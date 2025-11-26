using Domain.Map;
using Domain.Enums;

namespace Domain.Base
{
    public class Park : MapObject
    {
        public ParkType Type { get; }
        public int TreesCount { get; }

        public Park(Area area, ParkType type) : base(area)
        {
            Type = type;
            TreesCount = CalculateTreesCount(type, area);
        }

        private static int CalculateTreesCount(ParkType type, Area area)
        {
            return type switch
            {
                ParkType.BotanicalGarden => area.Width * area.Height * 5,
                ParkType.Square => area.Width * area.Height * 4,
                ParkType.UrbanPark => area.Width * area.Height * 3,
                _ => area.Width * area.Height * 2
            };
        }
    }
}
