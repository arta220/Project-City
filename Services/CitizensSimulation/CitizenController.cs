using Domain.Citizens;
using Domain.Common.Time;
using Domain.Map;
using Services.Citizens.Movement;
using System.Net.Http.Headers;

public class CitizenController
{
    private readonly ICitizenMovementService _movementService;
    public CitizenController(ICitizenMovementService movementService)
    {
        _movementService = movementService;
    }

    public void UpdateCitizen(Citizen citizen, SimulationTime time)
    {
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
    }
}
