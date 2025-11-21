using CitySkylines_REMAKE.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class TileVM : ObservableObject
    {
        private readonly TileModel _tileModel;

        public TileVM(TileModel tileModel)
        {
            _tileModel = tileModel;
        }
    }
}
