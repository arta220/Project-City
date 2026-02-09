using Domain.Base;
using Domain.Buildings;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.PlaceBuilding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Disasters
{
    public class DisasterService : IDisasterService
    {
        private readonly Random _random = new Random();
        private readonly IBuildingRegistry _buildingRegistry;
        private readonly PlacementRepository _placementRepository;
        private readonly MapModel _mapModel;
        private int _lastTick = 0;

        // Храним бедствия с временем окончания
        private readonly Dictionary<Building, Dictionary<DisasterType, (int StartTick, int Duration)>>
            _activeDisasters = new();

        // Статистика
        private readonly DisasterStatistics _statistics = new();

        // События
        public event Action<Building> BuildingDestroyed;

        // Длительность каждого бедствия в тиках
        private const int FIRE_DURATION = 50;
        private const int FLOOD_DURATION = 80;
        private const int BLIZZARD_DURATION = 100;

        public DisasterService(IBuildingRegistry buildingRegistry,
                              PlacementRepository placementRepository,
                              MapModel mapModel)
        {
            _buildingRegistry = buildingRegistry;
            _placementRepository = placementRepository;
            _mapModel = mapModel;
        }

        public void Update(SimulationTime time)
        {
            _lastTick = time.TotalTicks;

            var allBuildings = _buildingRegistry.GetBuildings<Building>().ToList();

            // Генерация новых бедствий - ИСПРАВЛЕННАЯ СЕКЦИЯ
            if (allBuildings.Count > 0 && _random.Next(100) < 3) // 3%
            {
                var disasterType = GetRandomDisasterType();

                // ВАЖНО: _random.Next(maxValue) возвращает от 0 до maxValue-1
                // но если maxValue = 0, то будет ArgumentOutOfRangeException
                var randomIndex = _random.Next(allBuildings.Count); // Используйте это значение

                var randomBuilding = allBuildings[randomIndex];

                if (!IsDisasterActive(randomBuilding, disasterType))
                {
                    StartDisaster(randomBuilding, disasterType, time.TotalTicks);
                }
            }


            // Обработка активных бедствий и нанесение урона
            var buildingsToRemove = new List<Building>();

            foreach (var buildingEntry in _activeDisasters.ToList())
            {
                var building = buildingEntry.Key;

                // Проверяем, что здание все еще существует
                if (!allBuildings.Contains(building))
                {
                    buildingsToRemove.Add(building);
                    continue;
                }

                // Создаем копию для безопасного перебора
                var disastersCopy = buildingEntry.Value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                foreach (var disasterEntry in disastersCopy)
                {
                    var disasterType = disasterEntry.Key;
                    var (startTick, duration) = disasterEntry.Value;

                    // Наносим урон зданию
                    float damage = GetDamagePerTick(disasterType);

                    // Используем рефлексию для доступа к Health/TakeDamage
                    var healthProp = building.GetType().GetProperty("Health");
                    if (healthProp != null)
                    {
                        float currentHealth = (float)healthProp.GetValue(building);
                        float newHealth = Math.Max(0, currentHealth - damage);
                        healthProp.SetValue(building, newHealth);

                        // Проверяем, разрушено ли здание
                        if (newHealth <= 0f)
                        {
                            buildingsToRemove.Add(building);
                            BuildingDestroyed?.Invoke(building);
                            break; // Прекращаем обработку этого здания
                        }
                    }
                }
            }

            // Удаляем разрушенные здания
            foreach (var building in buildingsToRemove)
            {
                EndAllDisasters(building);
            }

            // Проверяем окончание бедствий
            CheckForDisasterEnd(time.TotalTicks);

            // Обновляем статистику
            UpdateStatistics(time.TotalTicks);
        }

        private bool IsBuildingDestroyed(Building building)
        {
            var healthProp = building.GetType().GetProperty("Health");
            if (healthProp != null)
            {
                float health = (float)healthProp.GetValue(building);
                return health <= 0f;
            }
            return false;
        }

        private DisasterType GetRandomDisasterType()
        {
            var roll = _random.Next(100);
            if (roll < 40) return DisasterType.Fire;      // 40%
            if (roll < 70) return DisasterType.Flood;     // 30%
            return DisasterType.Blizzard;                // 30%
        }

        private float GetDamagePerTick(DisasterType type)
        {
            return type switch
            {
                DisasterType.Fire => 2.0f,
                DisasterType.Flood => 1.5f,
                DisasterType.Blizzard => 0.5f,
                _ => 1.0f
            };
        }

        private void StartDisaster(Building building, DisasterType disasterType, int currentTick)
        {
            if (IsDisasterActive(building, disasterType)) return;

            building.Disasters.StartDisaster(disasterType, currentTick);

            if (!_activeDisasters.ContainsKey(building))
                _activeDisasters[building] = new Dictionary<DisasterType, (int, int)>();

            int duration = GetDisasterDuration(disasterType);
            _activeDisasters[building][disasterType] = (currentTick, duration);
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

        private bool IsDisasterActive(Building building, DisasterType disasterType)
        {
            return _activeDisasters.ContainsKey(building) &&
                   _activeDisasters[building].ContainsKey(disasterType);
        }

        private void CheckForDisasterEnd(int currentTick)
        {
            var disastersToRemove = new List<(Building, DisasterType)>();

            foreach (var buildingEntry in _activeDisasters)
            {
                foreach (var disasterEntry in buildingEntry.Value)
                {
                    var (startTick, duration) = disasterEntry.Value;

                    if (currentTick - startTick >= duration)
                    {
                        disastersToRemove.Add((buildingEntry.Key, disasterEntry.Key));
                    }
                }
            }

            foreach (var (building, disasterType) in disastersToRemove)
            {
                EndDisaster(building, disasterType);
            }
        }

        private void EndAllDisasters(Building building)
        {
            if (_activeDisasters.TryGetValue(building, out var disasters))
            {
                foreach (var disasterType in disasters.Keys.ToList())
                {
                    building.Disasters.StopDisaster(disasterType);
                }
                _activeDisasters.Remove(building);
            }
        }

        public void FixDisaster(Building building, DisasterType disasterType)
        {
            EndDisaster(building, disasterType);
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

        public Dictionary<Building, List<DisasterType>> GetDisasterMap()
        {
            var result = new Dictionary<Building, List<DisasterType>>();

            foreach (var buildingEntry in _activeDisasters)
            {
                result[buildingEntry.Key] = buildingEntry.Value.Keys.ToList();
            }

            return result;
        }

        private void UpdateStatistics(int currentTick)
        {
            // Подсчитываем активные бедствия каждого типа
            int activeFires = 0;
            int activeFloods = 0;
            int activeBlizzards = 0;

            foreach (var buildingEntry in _activeDisasters)
            {
                foreach (var disasterEntry in buildingEntry.Value)
                {
                    switch (disasterEntry.Key)
                    {
                        case DisasterType.Fire:
                            activeFires++;
                            break;
                        case DisasterType.Flood:
                            activeFloods++;
                            break;
                        case DisasterType.Blizzard:
                            activeBlizzards++;
                            break;
                    }
                }
            }

            // Обновляем статистику
            _statistics.UpdateActiveDisasters(currentTick, activeFires, activeFloods, activeBlizzards);
        }

        public DisasterStatistics GetStatistics() => _statistics;
    }
}