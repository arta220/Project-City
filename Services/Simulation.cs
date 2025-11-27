using Domain.Base;
using Domain.Map;
using Services.Interfaces;
using Services.PlaceBuilding;
using Services.SimulationClock;
using System;
using System.Collections.ObjectModel;

namespace Services
{
    public class Simulation
    {
        private readonly IMapObjectPlacementService _placementService;
        private readonly ISimulationClock _clock;
        private readonly PlacementRepository _placementRepository;

        public event Action<int> TickOccurred;
        public event Action<MapObject> MapObjectPlaced;
        public event Action<MapObject> MapObjectRemoved;

        public MapModel MapModel { get; private set; }
        public ObservableCollection<MapObject> MapObjects { get; } = new ObservableCollection<MapObject>();

        public Simulation(
            MapModel mapModel,
            IMapObjectPlacementService placementService,
            ISimulationClock clock,
            PlacementRepository placementRepository)
        {
            MapModel = mapModel ?? throw new ArgumentNullException(nameof(mapModel));
            _placementService = placementService ?? throw new ArgumentNullException(nameof(placementService));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _placementRepository = placementRepository ?? throw new ArgumentNullException(nameof(placementRepository));

            _clock.TickOccurred += OnTick;
            _clock.Start();
        }

        private void OnTick(int tick)
        {
            TickOccurred?.Invoke(tick);
        }

        public bool TryPlace(MapObject mapObject, Placement placement)
        {
            if (_placementService.TryPlace(MapModel, mapObject, placement))
            {
                _placementRepository.Register(mapObject, placement);
                MapObjects.Add(mapObject);
                MapObjectPlaced?.Invoke(mapObject);
                return true;
            }
            return false;
        }

        public bool RemoveMapObject(MapObject mapObject)
        {
            //if (_placementRepository.Remove(mapObject))
            //{
            //    MapObjects.Remove(mapObject);
            //    MapObjectRemoved?.Invoke(mapObject);
            //    return true;
            //}
            return false;
        }

        public Placement GetMapObjectPlacement(MapObject mapObject)
        {
            return _placementRepository.GetPlacement(mapObject);
        }

        public bool CanPlace(MapObject mapObject, Placement placement)
        {
            return _placementService.CanPlace(MapModel, mapObject, placement);
        }
    }
}
