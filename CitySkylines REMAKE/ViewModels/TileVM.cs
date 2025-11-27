using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using Domain.Enums;
using Domain.Base;
using System.Windows.Threading;

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

    [ObservableProperty]
    private bool _isBlinkingRed; // Smirnov
    private DispatcherTimer _blinkTimer; // Smirnov

    public bool HasObject => TileModel.MapObject != null;
    
    public bool CanBuild => !HasObject;

    public TerrainType TerrainType => TileModel.Terrain;

    public MapObject MapObject => TileModel.MapObject;
    public TileVM(TileModel tileModel)
    {
        TileModel = tileModel;
        X = tileModel.Position.X;
        Y = tileModel.Position.Y;

        _blinkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
        _blinkTimer.Tick += (s, e) =>
        {
            if (IsBlinkingRed)
            {
                OnPropertyChanged(nameof(IsBlinkingRed)); // Принудительно обновляем привязку
            }
        };
        _blinkTimer.Start();

        TileModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TileModel.MapObject))
            {
                OnPropertyChanged(nameof(HasObject));
                UpdateBlinkingState();

                if (TileModel.MapObject is Domain.Base.Building building)
                {
                    building.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(building.HasBrokenUtilities))
                        {
                            UpdateBlinkingState();
                        }
                    };
                }
            }
        };

        UpdateBlinkingState(); // Smirnov
    }

    // Smirnov
    public void UpdateBlinkingState()
    {
        // Проверяем что это именно жилой дом и у него есть поломки
        if (TileModel.MapObject is Domain.Buildings.ResidentialBuilding residentialBuilding)
        {
            IsBlinkingRed = residentialBuilding.HasBrokenUtilities;
        }
        else
        {
            IsBlinkingRed = false;
        }
        OnPropertyChanged(nameof(IsBlinkingRed));
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
