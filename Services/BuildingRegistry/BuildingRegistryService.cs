using Domain.Base;
using Domain.Map;
using Services.PlaceBuilding;

namespace Services.BuildingRegistry
{
    public class BuildingRegistryService : IBuildingRegistry
    {
        private readonly PlacementRepository _placementRepository;
        public BuildingRegistryService(PlacementRepository placementRepository) 
        {
            _placementRepository = placementRepository;
        }
        public IEnumerable<MapObject> GetAllBuildings() => _placementRepository.GetAll();

        public Placement GetPlacement(MapObject building) => _placementRepository.GetPlacement(building);
    }
}
