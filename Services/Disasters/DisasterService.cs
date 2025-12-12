// Services/Disasters/DisasterService.cs
using Domain.Base;
using Domain.Buildings.Residential;
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

        // Храним заблокированные дороги для метели (для каждого здания с метелью)
        private readonly Dictionary<Building, List<Road>> _blockedRoads = new();

        // Длительность каждого бедствия в тиках
        private const int FIRE_DURATION = 50;     // 50 тиков
        private const int FLOOD_DURATION = 80;    // 80 тиков
        private const int BLIZZARD_DURATION = 100; // 100 тиков

        // Урон от бедствий за тик
        private const float FIRE_DAMAGE_PER_TICK = 1.5f;      // Пожар - самый опасный
        private const float FLOOD_DAMAGE_PER_TICK = 1.0f;     // Наводнение - средний урон
        private const float BLIZZARD_DAMAGE_PER_TICK = 0.5f;  // Метель - слабый урон

        /// <summary>
        /// Событие, которое вызывается при уничтожении здания.
        /// </summary>
        public event Action<Building> BuildingDestroyed;

        public DisasterService(IBuildingRegistry buildingRegistry, PlacementRepository placementRepository, MapModel mapModel)
        {
            _buildingRegistry = buildingRegistry;
            _placementRepository = placementRepository;
            _mapModel = mapModel;
        }

        public void Update(SimulationTime time)
        {
            _lastTick = time.TotalTicks;

            var allBuildings = _buildingRegistry.GetBuildings<Building>().ToList();

            if (allBuildings.Count > 0 && _random.Next(100) < 3) // 3%
            {
                var disasterType = GetRandomDisasterType();
                var randomBuilding = allBuildings[_random.Next(allBuildings.Count)];

                StartDisaster(randomBuilding, disasterType, time.TotalTicks);
            }

            // Наносим урон зданиям с активными бедствиями
            ApplyDamageToBuildings();

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

            // Применяем эффекты бедствия
            ApplyDisasterEffects(building, disasterType);

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

            // Отменяем эффекты бедствия
            RemoveDisasterEffects(building, disasterType);

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

        /// <summary>
        /// Наносит урон всем зданиям с активными бедствиями.
        /// </summary>
        private void ApplyDamageToBuildings()
        {
            var destroyedBuildings = new List<Building>();

            foreach (var buildingEntry in _activeDisasters.ToList())
            {
                var building = buildingEntry.Key;
                
                // Пропускаем уже уничтоженные здания
                if (building.IsDestroyed)
                    continue;

                // Суммируем урон от всех активных бедствий на здании
                float totalDamage = 0f;

                foreach (var disasterType in buildingEntry.Value.Keys)
                {
                    float damage = GetDisasterDamage(disasterType);
                    totalDamage += damage;
                }

                // Наносим урон
                building.TakeDamage(totalDamage);

                // Проверяем, уничтожено ли здание
                if (building.IsDestroyed)
                {
                    destroyedBuildings.Add(building);
                }
            }

            // Уведомляем об уничтоженных зданиях
            foreach (var destroyedBuilding in destroyedBuildings)
            {
                // Завершаем все бедствия на уничтоженном здании
                var disasters = _activeDisasters[destroyedBuilding].Keys.ToList();
                foreach (var disasterType in disasters)
                {
                    EndDisaster(destroyedBuilding, disasterType);
                }

                // Вызываем событие
                BuildingDestroyed?.Invoke(destroyedBuilding);
            }
        }

        /// <summary>
        /// Получает величину урона за тик для указанного типа бедствия.
        /// </summary>
        private float GetDisasterDamage(DisasterType type)
        {
            return type switch
            {
                DisasterType.Fire => FIRE_DAMAGE_PER_TICK,
                DisasterType.Flood => FLOOD_DAMAGE_PER_TICK,
                DisasterType.Blizzard => BLIZZARD_DAMAGE_PER_TICK,
                _ => 0f
            };
        }

        /// <summary>
        /// Применяет эффекты бедствия к зданию.
        /// </summary>
        private void ApplyDisasterEffects(Building building, DisasterType disasterType)
        {
            switch (disasterType)
            {
                case DisasterType.Fire:
                    // Пожар отключает электричество и газ
                    if (building is ResidentialBuilding residentialFire)
                    {
                        residentialFire.Utilities.BreakUtility(UtilityType.Electricity);
                        residentialFire.Utilities.BreakUtility(UtilityType.Gas);
                    }
                    break;

                case DisasterType.Flood:
                    // Наводнение ломает водопровод
                    if (building is ResidentialBuilding residentialFlood)
                    {
                        residentialFlood.Utilities.BreakUtility(UtilityType.Water);
                    }
                    break;

                case DisasterType.Blizzard:
                    // Метель блокирует дороги в области здания
                    BlockRoadsNearBuilding(building);
                    break;
            }
        }

        /// <summary>
        /// Отменяет эффекты бедствия.
        /// </summary>
        private void RemoveDisasterEffects(Building building, DisasterType disasterType)
        {
            switch (disasterType)
            {
                case DisasterType.Fire:
                    // Электричество и газ НЕ восстанавливаются автоматически
                    break;

                case DisasterType.Flood:
                    // Водопровод НЕ восстанавливается автоматически
                    break;

                case DisasterType.Blizzard:
                    // Разблокируем дороги
                    UnblockRoadsNearBuilding(building);
                    break;
            }
        }

        /// <summary>
        /// Блокирует дороги в области здания и вокруг него (для метели).
        /// </summary>
        private void BlockRoadsNearBuilding(Building building)
        {
            var (placement, found) = _buildingRegistry.TryGetPlacement(building);
            if (!found || placement == null) return;

            var blockedRoads = new List<Road>();
            var affectedArea = new Placement(
                new Position(placement.Value.Left - 1, placement.Value.Top - 1),
                new Area(placement.Value.Area.Width + 2, placement.Value.Area.Height + 2)
            );

            // Находим все дороги в расширенной области
            foreach (var pos in affectedArea.GetAllPositions())
            {
                if (pos.X >= 0 && pos.X < _mapModel.Width && pos.Y >= 0 && pos.Y < _mapModel.Height)
                {
                    var tile = _mapModel[pos];
                    if (tile.MapObject is Road road && !road.IsBlocked)
                    {
                        road.IsBlocked = true;
                        blockedRoads.Add(road);
                    }
                }
            }

            if (blockedRoads.Count > 0)
            {
                _blockedRoads[building] = blockedRoads;
            }
        }

        /// <summary>
        /// Разблокирует дороги, заблокированные метелью для этого здания.
        /// </summary>
        private void UnblockRoadsNearBuilding(Building building)
        {
            if (_blockedRoads.TryGetValue(building, out var roads))
            {
                foreach (var road in roads)
                {
                    // Проверяем, что дорога всё ещё существует и заблокирована нашим бедствием
                    // (может быть уже удалена или заблокирована другим бедствием)
                    if (road != null && road.IsBlocked)
                    {
                        // Проверяем, нет ли других активных метелей в этой области
                        // Для простоты просто разблокируем, если нет других метелей на этой дороге
                        bool hasOtherBlizzard = false;
                        var (roadPlacement, roadFound) = _placementRepository.TryGetPlacement(road);
                        
                        if (roadFound && roadPlacement != null)
                        {
                            // Проверяем все здания рядом с этой дорогой
                            foreach (var otherBuilding in _activeDisasters.Keys)
                            {
                                if (otherBuilding == building || !otherBuilding.Disasters.IsDisasterActive(DisasterType.Blizzard))
                                    continue;

                                var (otherPlacement, otherFound) = _buildingRegistry.TryGetPlacement(otherBuilding);
                                if (otherFound && otherPlacement != null)
                                {
                                    var otherArea = new Placement(
                                        new Position(otherPlacement.Value.Left - 1, otherPlacement.Value.Top - 1),
                                        new Area(otherPlacement.Value.Area.Width + 2, otherPlacement.Value.Area.Height + 2)
                                    );
                                    
                                    if (otherArea.Contains(roadPlacement.Value.Position))
                                    {
                                        hasOtherBlizzard = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!hasOtherBlizzard)
                        {
                            road.IsBlocked = false;
                        }
                    }
                }
                _blockedRoads.Remove(building);
            }
        }
    }
}