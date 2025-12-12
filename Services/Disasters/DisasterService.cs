// Services/Disasters/DisasterService.cs
using Domain.Base;
using Domain.Buildings.Disaster;
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

        // Статистика бедствий
        private readonly Dictionary<DisasterType, int> _totalOccurrences
            = new Dictionary<DisasterType, int>
            {
                [DisasterType.Fire] = 0,
                [DisasterType.Flood] = 0,
                [DisasterType.Blizzard] = 0
            };

        private readonly DisasterStatistics _statistics = new();

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

        // В DisasterService.cs в методе Update добавляем в конец:
        public void Update(SimulationTime time)
        {
            _lastTick = time.TotalTicks;

            var allBuildings = _buildingRegistry.GetBuildings<Building>().ToList();

            System.Diagnostics.Debug.WriteLine($"[DisasterService] Update called at tick {time.TotalTicks}, buildings count: {allBuildings.Count}, active disasters: {_activeDisasters.Count}");

            // 3% шанс нового бедствия
            if (allBuildings.Count > 0 && _random.Next(100) < 3)
            {
                var disasterType = GetRandomDisasterType();
                var randomBuilding = allBuildings[_random.Next(allBuildings.Count)];

                System.Diagnostics.Debug.WriteLine($"[DisasterService] Attempting to start {disasterType} on building of type {randomBuilding.GetType().Name}");
                StartDisaster(randomBuilding, disasterType, time.TotalTicks);
            }

            // Наносим урон зданиям с активными бедствиями
            ApplyDamageToBuildings();

            // Проверяем окончание бедствий
            CheckForDisasterEnd(time.TotalTicks);

            // ===== ДОБАВЛЯЕМ ЭТО =====
            // Обновляем статистику активных бедствий каждый тик
            UpdateDisasterStatistics(time.TotalTicks);
            // ========================
        }

        public Dictionary<Building, List<DisasterType>> GetDisasterMap()
        {
            var result = new Dictionary<Building, List<DisasterType>>();

            foreach (var buildingEntry in _activeDisasters)
            {
                // Теперь на здании только одно бедствие
                var disasterType = buildingEntry.Value.Keys.FirstOrDefault();
                if (disasterType != default(DisasterType))
                {
                    result[buildingEntry.Key] = new List<DisasterType> { disasterType };
                }
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
            // Не стартуем, если уже есть любое бедствие на здании
            if (building.Disasters.HasDisaster)
            {
                System.Diagnostics.Debug.WriteLine($"[DisasterService] Building already has an active disaster, skipping {disasterType}");
                return;
            }

            // Останавливаем все предыдущие бедствия (на случай если что-то пошло не так)
            if (_activeDisasters.ContainsKey(building))
            {
                var existingDisaster = _activeDisasters[building].Keys.FirstOrDefault();
                if (existingDisaster != default(DisasterType))
                {
                    EndDisaster(building, existingDisaster);
                }
            }

            building.Disasters.StartDisaster(disasterType, currentTick);

            _activeDisasters[building] = new Dictionary<DisasterType, (int, int)>();

            // Определяем длительность
            int duration = GetDisasterDuration(disasterType);
            _activeDisasters[building][disasterType] = (currentTick, duration);

            // Применяем эффекты бедствия
            ApplyDisasterEffects(building, disasterType);

            // Логирование для отладки
            System.Diagnostics.Debug.WriteLine($"[DisasterService] Successfully started {disasterType} on building at tick {currentTick}, duration: {duration}, HasDisaster: {building.Disasters.HasDisaster}");
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

            // Удаляем бедствие из словаря
            if (_activeDisasters.ContainsKey(building))
            {
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
                // Теперь на здании может быть только одно бедствие
                var disasterEntry = disasters.FirstOrDefault();
                if (disasterEntry.Key != default(DisasterType))
                {
                    var (startTick, duration) = disasterEntry.Value;
                    int elapsed = _lastTick - startTick;
                    int remaining = Math.Max(0, duration - elapsed);
                    result[disasterEntry.Key] = remaining;
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

                // Теперь на здании может быть только одно бедствие
                var disasterType = buildingEntry.Value.Keys.FirstOrDefault();
                if (disasterType == default(DisasterType))
                    continue;

                float damage = GetDisasterDamage(disasterType);

                // Наносим урон
                building.TakeDamage(damage);

                // Проверяем, уничтожено ли здание
                if (building.IsDestroyed)
                {
                    destroyedBuildings.Add(building);
                }
            }

            // Уведомляем об уничтоженных зданиях
            foreach (var destroyedBuilding in destroyedBuildings)
            {
                // Завершаем бедствие на уничтоженном здании (теперь только одно)
                var disasterType = _activeDisasters[destroyedBuilding].Keys.FirstOrDefault();
                if (disasterType != default(DisasterType))
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
            System.Diagnostics.Debug.WriteLine($"[DisasterService] Applying effects for {disasterType} on building");
            
            switch (disasterType)
            {
                case DisasterType.Fire:
                    // Пожар отключает электричество и газ
                    if (building is ResidentialBuilding residentialFire)
                    {
                        residentialFire.Utilities.BreakUtility(UtilityType.Electricity);
                        residentialFire.Utilities.BreakUtility(UtilityType.Gas);
                        System.Diagnostics.Debug.WriteLine($"[DisasterService] Fire: Electricity and Gas broken");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[DisasterService] Fire: Building is not ResidentialBuilding, skipping utility effects");
                    }
                    break;

                case DisasterType.Flood:
                    // Наводнение ломает водопровод
                    if (building is ResidentialBuilding residentialFlood)
                    {
                        residentialFlood.Utilities.BreakUtility(UtilityType.Water);
                        System.Diagnostics.Debug.WriteLine($"[DisasterService] Flood: Water broken");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[DisasterService] Flood: Building is not ResidentialBuilding, skipping utility effects");
                    }
                    break;

                case DisasterType.Blizzard:
                    // Метель блокирует дороги в области здания
                    System.Diagnostics.Debug.WriteLine($"[DisasterService] Blizzard: Blocking roads");
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

        // Новый метод для обновления статистики
        private void UpdateDisasterStatistics(int currentTick)
        {
            // Считаем активные бедствия
            int activeFireCount = 0;
            int activeFloodCount = 0;
            int activeBlizzardCount = 0;

            foreach (var buildingEntry in _activeDisasters)
            {
                // Теперь на здании только одно бедствие
                var disasterType = buildingEntry.Value.Keys.FirstOrDefault();
                if (disasterType == default(DisasterType)) continue;

                switch (disasterType)
                {
                    case DisasterType.Fire:
                        activeFireCount++;
                        break;
                    case DisasterType.Flood:
                        activeFloodCount++;
                        break;
                    case DisasterType.Blizzard:
                        activeBlizzardCount++;
                        break;
                }
            }

            // Обновляем статистику
            _statistics.UpdateActiveDisasters(currentTick, activeFireCount, activeFloodCount, activeBlizzardCount);
        }

        /// <summary>
        /// Обновляет статистику для указанного типа бедствия.
        /// </summary>
        private void UpdateStatistics(DisasterType disasterType, int currentTick)
        {
            var dataPoint = new DisasterDataPoint(currentTick, _totalOccurrences[disasterType]);

            switch (disasterType)
            {
                case DisasterType.Fire:
                    _statistics.FireHistory.Add(dataPoint);
                    break;
                case DisasterType.Flood:
                    _statistics.FloodHistory.Add(dataPoint);
                    break;
                case DisasterType.Blizzard:
                    _statistics.BlizzardHistory.Add(dataPoint);
                    break;
            }
        }

        /// <summary>
        /// Получает статистику по бедствиям.
        /// </summary>
        public DisasterStatistics GetStatistics() => _statistics;
    }
}

