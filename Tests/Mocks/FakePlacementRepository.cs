using Domain.Common.Base;
using Domain.Map;
using Services.PlaceBuilding;
using System.Collections.Generic;

namespace Tests.Mocks
{
    public class FakePlacementRepository : PlacementRepository
    {
        public FakePlacementRepository() : base()
        {
        }

        public new void Register(MapObject obj, Placement placement)
        {
            // В тесте просто добавляем в базовую коллекцию
            // Реальная логика не важна для unit тестов
        }

        public new void Unregister(MapObject obj)
        {
            // В тесте просто удаляем из базовой коллекции
        }

        public new IEnumerable<Placement> GetAllPlacements()
        {
            return new List<Placement>();
        }

        public new (Placement? placement, bool found) TryGetPlacement(MapObject obj)
        {
            return (new Placement(0, 0, obj.Area), true);
        }
    }
}
