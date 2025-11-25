using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;
using Domain.Enums;

namespace Domain.Map
{
    public class TileModel : ObservableObject
    {
        public Position Position { get; set; }
        public TerrainType Terrain { get; set; }
        private MapObject _mapObject;

        public MapObject MapObject
        {
            get => _mapObject;
            set => SetProperty(ref _mapObject, value);
        }
        public float Height { get; set; }

        public bool CanPlace(MapObject mapObject)
        {
            return MapObject == null;
        }
    }
}
