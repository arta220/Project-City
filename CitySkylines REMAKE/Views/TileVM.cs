using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using Domain.Enums;

namespace CitySimulatorWPF.ViewModels;

public partial class TileVM : ObservableObject
{
    public event Action<TileVM> TileClicked;
    public TileModel TileModel { get; }

    [ObservableProperty]
    public int _x;

    [ObservableProperty]
    public int _y;

    public TerrainType TerrainType => TileModel.Terrain;

    public TileVM(TileModel tileModel)
    {
        TileModel = tileModel;
        X = tileModel.Position.X;
        Y = tileModel.Position.Y;
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
