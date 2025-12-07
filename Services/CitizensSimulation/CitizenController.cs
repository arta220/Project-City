using Domain.Buildings.Residential;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Citizens.Tasks;
using Domain.Common.Time;
using Domain.Map;
using Services.BuildingRegistry;
using Services.Citizens.Job;
using Services.Citizens.Movement;
using Services.Utilities;
using System.Diagnostics;

public class CitizenController
{
    private readonly ICitizenMovementService _movementService;
    private readonly IBuildingRegistry _buildingRegistry; // ДОБАВИТЬ
    private readonly JobController _jobController; // ДОБАВИТЬ если нужно
    private readonly IUtilityService _utilityService;

    public CitizenController(
        ICitizenMovementService movementService,
        IBuildingRegistry buildingRegistry,
        JobController jobController,
        IUtilityService utilityService) 
    {
        _movementService = movementService;
        _buildingRegistry = buildingRegistry;
        _jobController = jobController;
        _utilityService = utilityService;
    }

    private bool IsAtWork(Citizen citizen)
    {
        if (citizen.WorkPlace == null)
        {
            Debug.WriteLine($"У работника {citizen.Id} нет WorkPlace");
            return false;
        }

        var (placement, ok) = _buildingRegistry.TryGetPlacement(citizen.WorkPlace);

        if (!ok)
        {
            Debug.WriteLine($"WorkPlace работника {citizen.Id} не найден в реестре");
            return false;
        }

        var entrance = placement.Value.Entrance;
        // ВЫВОДИМ РЕАЛЬНЫЕ КООРДИНАТЫ
        Debug.WriteLine($"Работник {citizen.Id}: Pos=({citizen.Position.X},{citizen.Position.Y}), Вход офиса=({entrance.X},{entrance.Y})");

        return citizen.Position == entrance;
    }

    public void UpdateCitizen(Citizen citizen, SimulationTime time)
    {
        if (citizen.Profession == CitizenProfession.UtilityWorker)
        {
            Debug.WriteLine($"UtilityWorker {citizen.Id}: State={citizen.State}");
        }

        if (citizen.CurrentTask == null)
        {
            if (citizen.Tasks.Count > 0)
                citizen.CurrentTask = citizen.Tasks.Dequeue();
        }

        if (citizen.CurrentTask != null && citizen.CurrentPath.Count == 0)
            _movementService.SetTarget(citizen, citizen.CurrentTask.Target);

        _movementService.PlayMovement(citizen, time);

        if (citizen.Position == citizen.CurrentTask?.Target)
        {
            citizen.CurrentTask.MarkAsCompleted();
            citizen.CurrentTask = null;
        }

        // ВЫЗОВ РАБОТЫ ДЛЯ UTILITYWORKER
        if (citizen.Profession == CitizenProfession.UtilityWorker)
        {
            if (citizen.State == CitizenState.Working)
            {
                Debug.WriteLine($"ВЫЗОВ РАБОТЫ: UtilityWorker {citizen.Id}");
                _jobController.UpdateJob(citizen, time);
            }
            else if (citizen.State == CitizenState.WorkingOnSite)
            {
                Debug.WriteLine($"ВЫЗОВ РАБОТЫ: UtilityWorker {citizen.Id} НА ВЫЕЗДЕ!");
                _jobController.UpdateJob(citizen, time);
            }
        }
    }
}