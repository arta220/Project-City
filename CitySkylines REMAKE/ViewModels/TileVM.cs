using CitySkylines_REMAKE.Models.Enums;
using CitySkylines_REMAKE.Models.Map;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class TileVM : ObservableObject
    {
        public event Action<TileVM> TileClicked;
        private readonly TileModel _tileModel;

        [ObservableProperty]
        public int _x;

        [ObservableProperty]
        public int _y;

        public TerrainType TerrainType => _tileModel.Terrain;

        public TileVM(TileModel tileModel)
        {
            _tileModel = tileModel;
        }

        [RelayCommand]
        public void TileClick() => TileClicked?.Invoke(this);

        [RelayCommand]
        public void TileLeave()
        {

        }
        [RelayCommand]
        public void TileEnter()
        {

        }
    }
}
