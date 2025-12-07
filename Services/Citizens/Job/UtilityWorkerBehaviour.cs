using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Services.Citizens.Job
{
    /// <summary>
    /// Вспомогательный класс для хранения информации о задании починки
    /// </summary>
    public class RepairAssignment
    {
        public ResidentialBuilding Building { get; }
        public UtilityType UtilityType { get; }
        public Position RepairPosition { get; }

        public RepairAssignment(ResidentialBuilding building, UtilityType utilityType, Position repairPosition)
        {
            Building = building;
            UtilityType = utilityType;
            RepairPosition = repairPosition;
        }
    }

    /// <summary>
    /// Поведение работника ЖКХ
    /// </summary>
    public class UtilityWorkerBehaviour : IJobBehaviour
    {
        public CitizenProfession CitizenProfession => CitizenProfession.UtilityWorker;

        private readonly IBuildingRegistry _buildingRegistry;
        private readonly IUtilityService _utilityService;
        private readonly Dictionary<Citizen, RepairAssignment> _currentAssignments = new();

        public UtilityWorkerBehaviour(IBuildingRegistry buildingRegistry, IUtilityService utilityService)
        {
            _buildingRegistry = buildingRegistry;
            _utilityService = utilityService;
        }

        // прверка всех состояний:
        public void Update(Citizen citizen, SimulationTime time)
        {
            if (citizen.Profession != CitizenProfession.UtilityWorker) return;
            Debug.WriteLine($"УтилитиВоркер: {citizen.Id}, Проф: {citizen.Profession}, State: {citizen.State}");

            // Если работник ДОШЕЛ до работы (в офисе)
            if (IsAtWork(citizen))
            {
                Debug.WriteLine($"DEBUG: Работник {citizen.Id} В ОФИСЕ!");
                TryAssignRepairTask(citizen);
            }
            // Если уже на выезде
            else if (citizen.State == CitizenState.WorkingOnSite)
            {
                Debug.WriteLine($"DEBUG: Работник {citizen.Id} на выезде");
                CheckRepairCompletion(citizen);
            }
            else
            {
                Debug.WriteLine($"DEBUG: Работник {citizen.Id} НЕ в офисе: State={citizen.State}, Position={citizen.Position}");
            }
        }

        private bool IsAtWork(Citizen citizen)
        {
            if (citizen.WorkPlace == null)
            {
                Debug.WriteLine($"DEBUG: У работника {citizen.Id} нет WorkPlace!");
                return false;
            }

            var (placement, ok) = _buildingRegistry.TryGetPlacement(citizen.WorkPlace);

            if (!ok)
            {
                Debug.WriteLine($"DEBUG: WorkPlace работника {citizen.Id} не найден в реестре!");
                return false;
            }

            var entrance = placement.Value.Entrance;
            var atWork = citizen.Position == entrance;

            Debug.WriteLine($"DEBUG: Работник {citizen.Id}: Pos={citizen.Position}, Вход офиса={entrance}, Совпадают? {atWork}");

            return atWork;
        }

        private bool IsAtRepairSite(Citizen citizen, Position repairPosition)
        {
            // Проверяем, находится ли работник на клетке рядом со зданием
            // Расстояние Манхэттена должно быть равно 1 (соседняя клетка)
            int distance = Math.Abs(citizen.Position.X - repairPosition.X) + 
                          Math.Abs(citizen.Position.Y - repairPosition.Y);
            return distance <= 1;
        }

        private bool IsAdjacent(Position pos1, Position pos2)
        {
            // Проверяем, являются ли две позиции соседними (расстояние Манхэттена = 1)
            int distance = Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y);
            return distance == 1;
        }

        private IEnumerable<Position> GetNeighbors(Position position)
        {
            // Возвращаем все соседние позиции (вверх, вниз, влево, вправо)
            return new List<Position>
            {
                new Position(position.X, position.Y - 1), // Вверх
                new Position(position.X, position.Y + 1), // Вниз
                new Position(position.X - 1, position.Y), // Влево
                new Position(position.X + 1, position.Y)  // Вправо
            };
        }

        private Position GetRepairPosition(MapObject building)
        {
            var (placement, ok) = _buildingRegistry.TryGetPlacement(building);
            if (!ok)
            {
                Debug.WriteLine($"GetRepairPosition: здание не найдено в реестре");
                return new Position(0, 0);
            }

            var entrance = placement.Value.Entrance;
            
            // Ищем первую доступную соседнюю позицию рядом со зданием
            // Проверяем все соседние позиции от входа
            foreach (var neighbor in GetNeighbors(entrance))
            {
                // Проверяем, что позиция не находится внутри здания
                if (!placement.Value.Contains(neighbor))
                {
                    Debug.WriteLine($"GetRepairPosition: вход={entrance}, ремонт={neighbor}");
                    return neighbor;
                }
            }

            // Если все соседние позиции заняты зданием, возвращаем позицию справа от входа
            var repairPos = new Position(entrance.X + 1, entrance.Y);
            Debug.WriteLine($"GetRepairPosition: вход={entrance}, ремонт={repairPos} (fallback)");
            return repairPos;
        }

        private void TryAssignRepairTask(Citizen citizen)
        {
            var assignment = FindRepairAssignment();
            if (assignment == null) return;

            citizen.State = CitizenState.WorkingOnSite;
            _currentAssignments[citizen] = assignment;

            // СТАВИМ ЗАДАЧУ СРАЗУ
            citizen.Tasks.Enqueue(new CitizenTask(
                CitizenTaskType.MoveToPosition,
                assignment.RepairPosition));

            Debug.WriteLine($"Работник {citizen.Id} отправлен чинить {assignment.UtilityType}, задача поставлена");
        }

        private RepairAssignment? FindRepairAssignment()
        {
            var residentialBuildings = _buildingRegistry.GetBuildings<ResidentialBuilding>().ToList();

            foreach (var building in residentialBuildings)
            {
                var brokenUtilities = _utilityService.GetBrokenUtilities(building);
                Debug.WriteLine($"Зданий: {residentialBuildings.Count}, Сломанных: {brokenUtilities.Count}");

                if (brokenUtilities.Any())
                {
                    var utilityType = brokenUtilities.First().Key;
                    var repairPosition = GetRepairPosition(building);
                    return new RepairAssignment(building, utilityType, repairPosition);
                }
            }

            return null;
        }

        private void CheckRepairCompletion(Citizen citizen)
        {
            if (!_currentAssignments.TryGetValue(citizen, out var assignment))
            {
                Debug.WriteLine($"CheckRepairCompletion: у работника {citizen.Id} нет задания");
                ReturnToOffice(citizen);
                return;
            }

            // Проверяем, находится ли работник рядом со зданием (на соседней клетке)
            var (placement, ok) = _buildingRegistry.TryGetPlacement(assignment.Building);
            if (!ok)
            {
                Debug.WriteLine($"CheckRepairCompletion: здание не найдено в реестре");
                _currentAssignments.Remove(citizen);
                ReturnToOffice(citizen);
                return;
            }

            var entrance = placement.Value.Entrance;
            // Проверяем, находится ли работник на позиции ремонта или рядом с входом
            bool isAtRepairPosition = citizen.Position == assignment.RepairPosition;
            bool isAdjacent = IsAdjacent(citizen.Position, entrance);
            
            // Также проверяем, дошел ли работник до цели задачи (если задача еще активна)
            bool isAtTaskTarget = citizen.CurrentTask != null && citizen.Position == citizen.CurrentTask.Target;

            Debug.WriteLine($"CheckRepairCompletion: Работник {citizen.Id} - Pos=({citizen.Position.X},{citizen.Position.Y}), " +
                          $"RepairPos=({assignment.RepairPosition.X},{assignment.RepairPosition.Y}), " +
                          $"Entrance=({entrance.X},{entrance.Y}), " +
                          $"isAtRepairPosition={isAtRepairPosition}, isAdjacent={isAdjacent}, isAtTaskTarget={isAtTaskTarget}, " +
                          $"Tasks.Count={citizen.Tasks.Count}, CurrentTask={citizen.CurrentTask != null}");

            // Если работник на позиции ремонта или рядом со зданием - проверяем, можно ли чинить
            bool isNearBuilding = isAtRepairPosition || isAdjacent || isAtTaskTarget;
            
            if (isNearBuilding)
            {
                // Проверяем, завершена ли задача движения (либо задача завершена, либо работник на цели)
                bool taskCompleted = citizen.CurrentTask == null || isAtTaskTarget;
                bool noMoreTasks = citizen.Tasks.Count == 0;
                
                Debug.WriteLine($"CheckRepairCompletion: Работник {citizen.Id} рядом со зданием. " +
                              $"taskCompleted={taskCompleted}, noMoreTasks={noMoreTasks}");
                
                if (taskCompleted && noMoreTasks)
                {
                    // Чиним только сломанные коммуналки
                    var brokenUtilities = _utilityService.GetBrokenUtilities(assignment.Building);
                    Debug.WriteLine($"CheckRepairCompletion: Работник {citizen.Id} начинает ремонт. Сломанных коммуналок: {brokenUtilities.Count}");
                    
                    if (brokenUtilities.Count == 0)
                    {
                        Debug.WriteLine($"CheckRepairCompletion: У здания нет сломанных коммуналок, возвращаемся в офис");
                        _currentAssignments.Remove(citizen);
                        ReturnToOffice(citizen);
                        return;
                    }

                    foreach (var utilityType in brokenUtilities.Keys)
                    {
                        _utilityService.FixUtility(assignment.Building, utilityType);
                        Debug.WriteLine($"Работник {citizen.Id} починил {utilityType} в здании");
                    }

                    _currentAssignments.Remove(citizen);
                    ReturnToOffice(citizen);
                }
                else
                {
                    Debug.WriteLine($"CheckRepairCompletion: Работник {citizen.Id} рядом, но задача еще не завершена: " +
                                  $"Tasks.Count={citizen.Tasks.Count}, CurrentTask={citizen.CurrentTask != null}, isAtTaskTarget={isAtTaskTarget}");
                }
            }
        }

        private void ReturnToOffice(Citizen citizen)
        {
            citizen.State = CitizenState.Working;
            if (citizen.WorkPlace != null)
            {
                var (placement, ok) = _buildingRegistry.TryGetPlacement(citizen.WorkPlace);
                if (ok)
                {
                    citizen.Tasks.Enqueue(new CitizenTask(
                        CitizenTaskType.MoveToPosition,
                        placement.Value.Entrance));
                }
            }
        }
    }
}