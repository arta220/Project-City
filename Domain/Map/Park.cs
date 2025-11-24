using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Enums;

namespace Domain.Map
{
    public class Park : MapObject
    {
        public string Name { get; set; }
        public ParkType Type { get; set; }
        public int TreesCount { get; set; }
        public bool HasPlayground { get; set; }
        public bool HasFountain { get; set; }

        public Park(Area area, string name, ParkType type) : base(area)
        {
            Name = name;
            Type = type;
            TreesCount = CalculateTreesCount(type, area);
            HasPlayground = type == ParkType.Playground || type == ParkType.Square;
            HasFountain = type == ParkType.UrbanPark || type == ParkType.BotanicalGarden;
        }

        private int CalculateTreesCount(ParkType type, Area area)
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
