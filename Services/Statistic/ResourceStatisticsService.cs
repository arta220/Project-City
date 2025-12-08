using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Statistic
{
    /// <summary>
    /// Сервис, собирающий статистику по суммарному количеству ресурсов на карте.
    /// Раз в тик пробегает по карте и суммирует Wood/Coal/Oil/Gas/...
    /// </summary>
    public interface IResourceStatisticsService : IUpdatable
    {
        ResourceStatistics GetStatistics();
    }

    public class ResourceStatisticsService : IResourceStatisticsService
    {
        private readonly MapModel _map;
        private readonly ResourceStatistics _statistics = new();

        public ResourceStatisticsService(MapModel map)
        {
            _map = map;
        }

        public ResourceStatistics GetStatistics() => _statistics;

        /// <summary>
        /// Вызывается из Simulation на каждом тике.
        /// </summary>
        public void Update(SimulationTime time)
        {
            var snapshot = new ResourceSnapshot
            {
                Tick = time.TotalTicks // или time.TotalTicks, как у тебя называется
            };

            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    var tile = _map[x, y];

                    switch (tile.ResourceType)
                    {
                        case NaturalResourceType.Iron:
                            snapshot.Iron += tile.ResourceAmount;
                            break;
                        case NaturalResourceType.Copper:
                            snapshot.Copper += tile.ResourceAmount;
                            break;
                        case NaturalResourceType.Oil:
                            snapshot.Oil += tile.ResourceAmount;
                            break;
                        case NaturalResourceType.Gas:
                            snapshot.Gas += tile.ResourceAmount;
                            break;
                        case NaturalResourceType.Wood:
                            snapshot.Wood += tile.ResourceAmount;
                            break;
                    }
                }
            }

            _statistics.History.Add(snapshot);
        }
    }
}
