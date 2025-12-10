using Domain.Map;
using Domain.Common.Enums;
using Domain.Common.Base;

namespace Domain.Base
{
    public class Park : MapObject
    {
        public ParkType Type { get; }
        public int TreesCount { get; }

        public TerrainType TerrainType =>
            Type switch
            {
                ParkType.UrbanPark => TerrainType.UrbanPark,
                ParkType.BotanicalGarden => TerrainType.BotanicalGarden,
                ParkType.Playground => TerrainType.Playground,
                ParkType.Square => TerrainType.Square,
                ParkType.RecreationArea => TerrainType.RecreationArea,
                _ => TerrainType.Plain
            };

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
