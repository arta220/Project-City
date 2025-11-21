using CitySkylines_REMAKE.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class TileVM : ObservableObject
    {
        public event Action<TileVM> TileClicked;
        private readonly TileModel _tileModel;

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
