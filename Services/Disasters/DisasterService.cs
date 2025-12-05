// Services/Disasters/DisasterService.cs
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Time;
using Services.BuildingRegistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Disasters
{
    public class DisasterService : IDisasterService
    {
        private readonly Random _random = new Random();
        private readonly IBuildingRegistry _buildingRegistry;
        private int _lastTick = 0;

        // Храним бедствия с временем окончания
        private readonly Dictionary<Building, Dictionary<DisasterType, (int StartTick, int Duration)>>
            _activeDisasters = new();

        // Длительность каждого бедствия в тиках
        private const int FIRE_DURATION = 50;     // 50 тиков
        private const int FLOOD_DURATION = 80;    // 80 тиков
        private const int BLIZZARD_DURATION = 100; // 100 тиков

        public DisasterService(IBuildingRegistry buildingRegistry)
        {
            _buildingRegistry = buildingRegistry;
        }

        public void Update(SimulationTime time)
        {
            _lastTick = time.TotalTicks;

            var allBuildings = _buildingRegistry.GetBuildings<Building>().ToList();

            if (_random.Next(100) < 3) // 3%
            {
                var disasterType = GetRandomDisasterType();
                var randomBuilding = allBuildings[_random.Next(allBuildings.Count)];

                StartDisaster(randomBuilding, disasterType, time.TotalTicks);
            }

            // Проверяем окончание бедствий
            CheckForDisasterEnd(time.TotalTicks);
        }
        public Dictionary<Building, List<DisasterType>> GetDisasterMap()
        {
            var result = new Dictionary<Building, List<DisasterType>>();

            foreach (var buildingEntry in _activeDisasters)
            {
                result[buildingEntry.Key] = buildingEntry.Value.Keys.ToList();
            }

            return result;
        }

        private DisasterType GetRandomDisasterType()
        {
            var roll = _random.Next(100);
            if (roll < 40) return DisasterType.Fire;      // 40%
            if (roll < 70) return DisasterType.Flood;     // 30%
            return DisasterType.Blizzard;                // 30%
        }

        private void StartDisaster(Building building, DisasterType disasterType, int currentTick)
        {
            // Не стартуем, если уже есть это бедствие
            if (IsDisasterActive(building, disasterType)) return;

            building.Disasters.StartDisaster(disasterType, currentTick);

            if (!_activeDisasters.ContainsKey(building))
                _activeDisasters[building] = new Dictionary<DisasterType, (int, int)>();

            // Определяем длительность
            int duration = GetDisasterDuration(disasterType);
            _activeDisasters[building][disasterType] = (currentTick, duration);

            // Логирование для отладки
            System.Diagnostics.Debug.WriteLine($"[DisasterService] Started {disasterType} on building at tick {currentTick}, duration: {duration}");
        }

        private int GetDisasterDuration(DisasterType type)
        {
            return type switch
            {
                DisasterType.Fire => FIRE_DURATION,
                DisasterType.Flood => FLOOD_DURATION,
                DisasterType.Blizzard => BLIZZARD_DURATION,
                _ => 50
            };
        }

        private void CheckForDisasterEnd(int currentTick)
        {
            var disastersToRemove = new List<(Building, DisasterType)>();

            foreach (var buildingEntry in _activeDisasters)
            {
                foreach (var disasterEntry in buildingEntry.Value)
                {
                    var (startTick, duration) = disasterEntry.Value;

                    // Проверяем, закончилось ли бедствие
                    if (currentTick - startTick >= duration)
                    {
                        disastersToRemove.Add((buildingEntry.Key, disasterEntry.Key));
                    }
                }
            }

            // Завершаем бедствия
            foreach (var (building, disasterType) in disastersToRemove)
            {
                EndDisaster(building, disasterType);
            }
        }

        private void EndDisaster(Building building, DisasterType disasterType)
        {
            building.Disasters.StopDisaster(disasterType);

            if (_activeDisasters.TryGetValue(building, out var dict))
            {
                dict.Remove(disasterType);
                if (dict.Count == 0)
                    _activeDisasters.Remove(building);
            }

            // Логирование для отладки
            System.Diagnostics.Debug.WriteLine($"[DisasterService] Ended {disasterType} on building");
        }

        public Dictionary<DisasterType, int> GetActiveDisasters(Building building)
        {
            var result = new Dictionary<DisasterType, int>();

            if (_activeDisasters.TryGetValue(building, out var disasters))
            {
                foreach (var entry in disasters)
                {
                    var (startTick, duration) = entry.Value;
                    int elapsed = _lastTick - startTick;
                    int remaining = Math.Max(0, duration - elapsed);
                    result[entry.Key] = remaining;
                }
            }

            return result;
        }

        public void FixDisaster(Building building, DisasterType disasterType)
        {
            // Завершаем бедствие вручную
            EndDisaster(building, disasterType);

            System.Diagnostics.Debug.WriteLine($"[DisasterService] Manually fixed {disasterType} on building");
        }

        private bool IsDisasterActive(Building building, DisasterType disasterType)
        {
            return _activeDisasters.ContainsKey(building) &&
                   _activeDisasters[building].ContainsKey(disasterType);
        }
    }
}