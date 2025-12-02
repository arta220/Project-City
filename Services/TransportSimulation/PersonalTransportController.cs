using Domain.Common.Base;
using Domain.Common.Time;
using Domain.Transports.Ground;
using Services.TransportSimulation.StateHandlers;

namespace Services.TransportSimulation;
public class PersonalTransportController
{
    private readonly IEnumerable<ITransportStateHandler> _stateHandlers;

    public PersonalTransportController(IEnumerable<ITransportStateHandler> stateHandlers)
    {
        _stateHandlers = stateHandlers;
    }

    public void UpdateTransport(PersonalCar car, SimulationTime time)
    {
        var handler = _stateHandlers.FirstOrDefault(h => h.CanHandle(car.State));
        handler?.Update(car, time);
    }
}
