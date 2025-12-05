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
        private const int MaxFactoryInfluence = 5;    // максимум учитываемых заводов вокруг клетки

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

            // 1. Собираем все тайлы с заводами
            var factoryTiles = new List<TileModel>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var t = _map[x, y];
                    if (IsFactoryTile(t))
                        factoryTiles.Add(t);
                }
            }

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

                    // --- считаем, сколько заводов в радиусе ---
                    int factoryCount = 0;

                    foreach (var factory in factoryTiles)
                    {
                        int dx = tile.Position.X - factory.Position.X;
                        int dy = tile.Position.Y - factory.Position.Y;
                        int distSq = dx * dx + dy * dy;

                        if (distSq <= radiusSq)
                            factoryCount++;
                    }

                    factoryCount = Math.Min(factoryCount, MaxFactoryInfluence);

                    // ===== 1. ВЫРУБКА ЛЕСА (если есть заводы и дерево > 0) =====
                    if (factoryCount > 0 && tile.ResourceAmount > 0)
                    {
                        float loss = WoodLossPerTick * factoryCount;
                        tile.ResourceAmount -= loss;

                        if (tile.ResourceAmount < 0)
                            tile.ResourceAmount = 0;

                        // при активной вырубке таймер восстановления сбрасываем
                        tile.DepletedTick = null;
                    }

                    // ===== 2. ВОССТАНОВЛЕНИЕ С НУЛЯ =====
                    // Условие: ресурс == 0. Восстанавливаем через RegenDelayTicks
                    // даже если заводы рядом остались.
                    if (tile.ResourceAmount <= 0)
                    {
                        if (!tile.DepletedTick.HasValue)
                            tile.DepletedTick = currentTick;

                        int ticksSince = currentTick - tile.DepletedTick.Value;

                        if (ticksSince >= tile.RegenDelayTicks)
                        {
                            tile.ResourceAmount = tile.MaxResourceAmount;
                            tile.DepletedTick = null;
                        }

                        // ресурс 0 — дальше этот тайл сейчас не трогаем
                        continue;
                    }

                    // ===== 3. ВОССТАНОВЛЕНИЕ, ЕСЛИ ЛЕС ПОКАЛЕЧЕН И ЗАВОДОВ НЕТ =====
                    // Условие: 0 < ресурс < максимум, рядом НЕТ заводов.
                    if (factoryCount == 0 &&
                        tile.ResourceAmount < tile.MaxResourceAmount)
                    {
                        if (!tile.DepletedTick.HasValue)
                            tile.DepletedTick = currentTick;

                        int ticksSince = currentTick - tile.DepletedTick.Value;

                        if (ticksSince >= tile.RegenDelayTicks)
                        {
                            tile.ResourceAmount = tile.MaxResourceAmount;
                            tile.DepletedTick = null;
                        }
                    }
                    else
                    {
                        // либо лес полный, либо рядом снова появились заводы —
                        // таймер восстановления сбрасываем
                        if (tile.ResourceAmount >= tile.MaxResourceAmount ||
                            factoryCount > 0)
                        {
                            tile.DepletedTick = null;
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
