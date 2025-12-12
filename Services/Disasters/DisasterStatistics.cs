// DisasterStatistics.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Disasters
{
    public class DisasterStatistics
    {
        public List<DisasterDataPoint> FireHistory { get; set; } = new();
        public List<DisasterDataPoint> FloodHistory { get; set; } = new();
        public List<DisasterDataPoint> BlizzardHistory { get; set; } = new();

        // Храним историю активных бедствий по тикам
        private readonly Dictionary<int, int> _activeFiresByTick = new();
        private readonly Dictionary<int, int> _activeFloodsByTick = new();
        private readonly Dictionary<int, int> _activeBlizzardsByTick = new();

        private int _lastTick = 0;
        private const int MAX_HISTORY_POINTS = 1000; // Максимальное количество точек в истории

        /// <summary>
        /// Обновляет статистику активных бедствий на текущий тик
        /// </summary>
        public void UpdateActiveDisasters(int currentTick,
            int activeFireCount, int activeFloodCount, int activeBlizzardCount)
        {
            _lastTick = currentTick;

            // Записываем количество активных бедствий на этот тик
            _activeFiresByTick[currentTick] = activeFireCount;
            _activeFloodsByTick[currentTick] = activeFloodCount;
            _activeBlizzardsByTick[currentTick] = activeBlizzardCount;

            // Ограничиваем размер истории
            CleanupOldHistory();

            // Обновляем списки для графика
            UpdateHistoryLists();
        }

        private void CleanupOldHistory()
        {
            // Удаляем старые точки, если их слишком много
            if (_activeFiresByTick.Count > MAX_HISTORY_POINTS)
            {
                var ticksToRemove = _activeFiresByTick.Keys
                    .OrderBy(t => t)
                    .Take(_activeFiresByTick.Count - MAX_HISTORY_POINTS)
                    .ToList();

                foreach (var tick in ticksToRemove)
                {
                    _activeFiresByTick.Remove(tick);
                    _activeFloodsByTick.Remove(tick);
                    _activeBlizzardsByTick.Remove(tick);
                }
            }
        }

        private void UpdateHistoryLists()
        {
            // Очищаем историю
            FireHistory.Clear();
            FloodHistory.Clear();
            BlizzardHistory.Clear();

            // Если нет данных, добавляем нулевую точку
            if (!_activeFiresByTick.Any())
            {
                FireHistory.Add(new DisasterDataPoint(_lastTick, 0));
                FloodHistory.Add(new DisasterDataPoint(_lastTick, 0));
                BlizzardHistory.Add(new DisasterDataPoint(_lastTick, 0));
                return;
            }

            // Находим диапазон тиков
            var allTicks = _activeFiresByTick.Keys
                .Union(_activeFloodsByTick.Keys)
                .Union(_activeBlizzardsByTick.Keys)
                .OrderBy(t => t)
                .ToList();

            if (!allTicks.Any()) return;

            int minTick = allTicks.First();
            int maxTick = Math.Max(_lastTick, allTicks.Last());

            // Для графика нужны все тики от min до max
            // Но если диапазон слишком большой, прореживаем
            int totalTicks = maxTick - minTick + 1;
            int step = totalTicks > 500 ? totalTicks / 500 + 1 : 1;

            for (int tick = minTick; tick <= maxTick; tick += step)
            {
                int fireCount = _activeFiresByTick.ContainsKey(tick) ? _activeFiresByTick[tick] : 0;
                int floodCount = _activeFloodsByTick.ContainsKey(tick) ? _activeFloodsByTick[tick] : 0;
                int blizzardCount = _activeBlizzardsByTick.ContainsKey(tick) ? _activeBlizzardsByTick[tick] : 0;

                FireHistory.Add(new DisasterDataPoint(tick, fireCount));
                FloodHistory.Add(new DisasterDataPoint(tick, floodCount));
                BlizzardHistory.Add(new DisasterDataPoint(tick, blizzardCount));
            }

            // Добавляем последнюю точку, если её нет
            if (!FireHistory.Any(p => p.Tick == _lastTick))
            {
                int fireCount = _activeFiresByTick.ContainsKey(_lastTick) ? _activeFiresByTick[_lastTick] : 0;
                int floodCount = _activeFloodsByTick.ContainsKey(_lastTick) ? _activeFloodsByTick[_lastTick] : 0;
                int blizzardCount = _activeBlizzardsByTick.ContainsKey(_lastTick) ? _activeBlizzardsByTick[_lastTick] : 0;

                FireHistory.Add(new DisasterDataPoint(_lastTick, fireCount));
                FloodHistory.Add(new DisasterDataPoint(_lastTick, floodCount));
                BlizzardHistory.Add(new DisasterDataPoint(_lastTick, blizzardCount));
            }
        }
    }
}