using Domain.Citizens;
using Domain.Common.Time;
using Services.CitizensSimulation.StateHandlers;

public class CitizenController
{
    private readonly IEnumerable<ICitizenStateHandler> _stateHandlers;

    public CitizenController(IEnumerable<ICitizenStateHandler> stateHandlers)
    {
        _stateHandlers = stateHandlers;
    }

    public void UpdateCitizen(Citizen citizen, SimulationTime time)
    {
        var handler = _stateHandlers.FirstOrDefault(h => h.CanHandle(citizen.State));
        handler?.Update(citizen, time);
    }
}
