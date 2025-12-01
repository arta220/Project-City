using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;

namespace Domain.Infrastructure
{
    /// <summary>
    /// Базовый класс для пешеходных и велодорожек
    /// </summary>
    public abstract class Path : MapObject
    {
        public PathType Type { get; }

        protected Path(PathType type) : base(new Area(1, 1))
        {
            Type = type;
        }
    }

    /// <summary>
    /// Пешеходная дорожка
    /// </summary>
    public class PedestrianPath : Path
    {
        public PedestrianPath() : base(PathType.Pedestrian)
        {
        }
    }


    /// <summary>
    /// Велосипедная дорожка
    /// </summary>
    public class BicyclePath : Path
    {
        public BicyclePath() : base(PathType.Bicycle)
        {
        }
    }
}
