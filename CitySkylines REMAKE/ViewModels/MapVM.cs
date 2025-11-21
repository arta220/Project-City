using CitySkylines_REMAKE.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class MapVM : ObservableObject
    {
        private readonly MapModel _mapModel;
        public ObservableCollection<TileVM> Tiles { get; set; }

        public MapVM()
        {
            Tiles = new();
        }
    }
}
