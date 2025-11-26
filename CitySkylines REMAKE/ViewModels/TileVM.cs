using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using Domain.Enums;
using Domain.Base;

namespace CitySimulatorWPF.ViewModels;

public partial class TileVM : ObservableObject
{
    public event Action<TileVM> TileClicked;
    public TileModel TileModel { get; }

    [ObservableProperty]
    public int _x;

    [ObservableProperty]
    public int _y;

    [ObservableProperty] // ДЛЯ ТЕСТА
    private bool _hasCitizen;


    public bool HasObject => TileModel.MapObject != null;

    public TerrainType TerrainType => TileModel.Terrain;

    public MapObject MapObject => TileModel.MapObject;
    public TileVM(TileModel tileModel)
    {
        TileModel = tileModel;
        X = tileModel.Position.X;
        Y = tileModel.Position.Y;

        TileModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TileModel.MapObject))
                OnPropertyChanged(nameof(HasObject));
        };
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
