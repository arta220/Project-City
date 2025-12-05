using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Domain.Buildings;
using Services.Time.Clock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Forest
{
    /// <summary>
    /// На каждом тике:
    /// - уменьшает ресурс дерева вокруг заводов;
    /// - после полного истощения ждёт задержку;
    /// - затем мгновенно восстанавливает дерево.
    /// </summary>
    public class ForestResourceService : IForestResourceService
    {
        private readonly MapModel _map;

        // внутренний счётчик тиков для леса (независим от SimulationTime)
        private int _localTick;

        // Настройки
        private const int FactoryRadius = 5;           // радиус влияния завода (тайлы)
        private const float WoodLossPerTick = 2f;      // сколько дерева теряется за тик

        public ForestResourceService(MapModel map)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        /// <summary>
        /// Вызывается из Simulation на каждом тике.
        /// </summary>
        public void Update(SimulationTime time)
        {
            _localTick++;
            UpdateForestResources(_localTick);
        }

        private void UpdateForestResources(int currentTick)
        {
            int width = _map.Width;
            int height = _map.Height;

            // 1. Собираем тайлы с заводами
            var factoryTiles = new List<TileModel>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = _map[x, y];

                    if (IsFactoryTile(tile))
                        factoryTiles.Add(tile);
                }
            }

            if (factoryTiles.Count == 0)
                return;

            int radiusSq = FactoryRadius * FactoryRadius;

            // 2. Обрабатываем лесные тайлы с деревом
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = _map[x, y];

                    if (tile.Terrain != TerrainType.Forest ||
                        tile.ResourceType != NaturalResourceType.Wood)
                        continue;

                    // Считаем, сколько заводов в радиусе
                    int factoryCount = 0;

                    foreach (var factory in factoryTiles)
                    {
                        int dx = tile.Position.X - factory.Position.X;
                        int dy = tile.Position.Y - factory.Position.Y;
                        int distSq = dx * dx + dy * dy;

                        if (distSq <= radiusSq)
                            factoryCount++;
                    }

                    // потолок влияния: максимум 3 заводов учитываем
                    const int MaxFactoryInfluence = 3;
                    factoryCount = Math.Min(factoryCount, MaxFactoryInfluence);

                    // Если рядом нет ни одного завода — пропускаем
                    if (factoryCount == 0)
                        continue;

                    // 2.1. Ресурс ещё есть — уменьшаем
                    if (tile.ResourceAmount > 0)
                    {
                        // скорость вырубки пропорциональна количеству заводов
                        float loss = WoodLossPerTick * factoryCount;
                        tile.ResourceAmount -= loss;

                        if (tile.ResourceAmount < 0)
                            tile.ResourceAmount = 0;

                        if (tile.ResourceAmount == 0 && tile.DepletedTick == null)
                            tile.DepletedTick = currentTick;

                        continue;
                    }

                    // 2.2. Ресурс исчерпан — ждём задержку и восстанавливаем СРАЗУ до максимума
                    if (tile.IsResourceDepleted && tile.DepletedTick.HasValue)
                    {
                        int ticksSinceDepleted = currentTick - tile.DepletedTick.Value;

                        if (ticksSinceDepleted >= tile.RegenDelayTicks)
                        {
                            tile.ResourceAmount = tile.MaxResourceAmount;
                            tile.DepletedTick = null; // цикл завершён
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Определяет, является ли объект на тайле заводом.
        /// </summary>
        private bool IsFactoryTile(TileModel tile)
        {
            // заводы и склады у тебя — это IndustrialBuilding
            return tile.MapObject is IndustrialBuilding;
        }
    }
}
