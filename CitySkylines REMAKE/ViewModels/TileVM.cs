using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using Domain.Enums;
using Domain.Base;

namespace CitySimulatorWPF.ViewModels;

public partial class TileVM : ObservableObject
{
    public event Action<TileVM> TileClicked;
    public event Action<TileVM> TileConstructionStart;
    public TileModel TileModel { get; }

    [ObservableProperty]
    public int _x;

    [ObservableProperty]
    public int _y;

    [ObservableProperty] // ДЛЯ ТЕСТА
    private bool _hasCitizen;

    [ObservableProperty]
    private bool _isPreviewTile = false;
    
    [ObservableProperty]
    private bool _isMouseOver = false;

    public bool HasObject => TileModel.MapObject != null;
    
    public bool CanBuild => !HasObject;

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
    public void TileMouseDown()
    {
        // Вызываем новое событие для MapVM, чтобы начать процесс строительства дороги
        TileConstructionStart?.Invoke(this); 
    }

    [RelayCommand]
    // Эта команда вызывается при покидании тайла
    public void TileLeave()
    {
        IsMouseOver = false;
    }
    
    [RelayCommand]
    // Эта команда вызывается при наведении на тайл и будет использоваться для обновления превью дороги (MouseMove)
    public void TileEnter()
    {
        IsMouseOver = true;
    }
}
