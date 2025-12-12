using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using System.Windows.Threading;
using Domain.Common.Enums;
using Domain.Common.Base;
using Domain.Buildings.Residential;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для отдельной клетки карты (<see cref="TileModel"/>).
    /// Содержит информацию о позиции, типе местности и размещённом объекте.
    /// </summary>
    /// <remarks>
    /// Контекст использования:
    /// - В <see cref="MapVM"/> для отображения и взаимодействия с клетками.
    /// - Обрабатывает клики, наведение мыши и начало строительства.
    /// </remarks>
    public partial class TileVM : ObservableObject
    {
        public event Action<TileVM> TileClicked;
        public event Action<TileVM> TileConstructionStart;

        public TileModel TileModel { get; }

        [ObservableProperty] private int _x;
        [ObservableProperty] private int _y;
        [ObservableProperty] private bool _isBlinkingRed;
        [ObservableProperty] private bool _isBlinkingBlue;
        [ObservableProperty] private bool _isPreviewTile = false;
        [ObservableProperty] private bool _isMouseOver = false;

        private DispatcherTimer _blinkTimer;
        private DispatcherTimer _disasterBlinkTimer;
        
        // Флаги для отслеживания состояния мигания
        private bool _shouldBlinkRed = false;
        private bool _shouldBlinkBlue = false;
        private bool _blinkRedState = false;
        private bool _blinkBlueState = false;

        public bool HasObject => TileModel.MapObject != null;
        public bool CanBuild => !HasObject;
        public TerrainType TerrainType => TileModel.Terrain;

        /// <summary>
        /// Тип природного ресурса на клетке.
        /// </summary>
        public NaturalResourceType ResourceType => TileModel.ResourceType;

        /// <summary>
        /// Есть ли на клетке природный ресурс.
        /// </summary>
        public bool HasResource => TileModel.ResourceType != NaturalResourceType.None;

        /// <summary>
        /// Количество природного ресурса на клетке.
        /// </summary>
        public float ResourceAmount => TileModel.ResourceAmount;

        /// <summary>
        /// Высота клетки (для подсказки/отладки рельефа).
        /// </summary>
        public float Height => TileModel.Height;

        /// <summary>
        /// Объект, размещённый на клетке (если есть).
        /// </summary>
        public MapObject MapObject => TileModel.MapObject;

        public TileVM(TileModel tileModel)
        {
            TileModel = tileModel ?? throw new ArgumentNullException(nameof(tileModel));
            X = tileModel.Position.X;
            Y = tileModel.Position.Y;

            _blinkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _blinkTimer.Tick += (s, e) =>
            {
                if (_shouldBlinkRed)
                {
                    _blinkRedState = !_blinkRedState;
                    IsBlinkingRed = _blinkRedState;
                }
            };
            _blinkTimer.Start();

            _disasterBlinkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _disasterBlinkTimer.Tick += (s, e) =>
            {
                if (_shouldBlinkBlue)
                {
                    _blinkBlueState = !_blinkBlueState;
                    IsBlinkingBlue = _blinkBlueState;
                }
            };
            _disasterBlinkTimer.Start();

            TileModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TileModel.MapObject))
                {
                    OnPropertyChanged(nameof(HasObject));
                    UpdateBlinkingState();

                    if (TileModel.MapObject is Building building)
                    {
                        building.Disasters.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(building.Disasters.HasDisaster))
                                UpdateBlinkingState();
                        };
                    }

                    if (TileModel.MapObject is ResidentialBuilding residential)
                    {
                        residential.Utilities.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(residential.Utilities.HasBrokenUtilities))
                                UpdateBlinkingState();
                        };
                    }
                }
            };

            UpdateBlinkingState();
        }

        public void UpdateBlinkingState()
        {
            bool hasBrokenUtilities = false;
            bool hasDisaster = false;

            if (TileModel.MapObject is ResidentialBuilding residential)
            {
                hasBrokenUtilities = residential.Utilities.HasBrokenUtilities;
            }

            if (TileModel.MapObject is Building building)
            {
                hasDisaster = building.Disasters.HasDisaster;
            }

            // Приоритет: бедствия (синий) важнее чем ЖКХ (красный)
            if (hasDisaster)
            {
                _shouldBlinkBlue = true;
                _shouldBlinkRed = false;
                // Сбрасываем состояние мигания, чтобы начать с нужной фазы
                _blinkBlueState = true;
                IsBlinkingBlue = true;
                IsBlinkingRed = false;
            }
            else if (hasBrokenUtilities)
            {
                _shouldBlinkBlue = false;
                _shouldBlinkRed = true;
                // Сбрасываем состояние мигания, чтобы начать с нужной фазы
                _blinkRedState = true;
                IsBlinkingBlue = false;
                IsBlinkingRed = true;
            }
            else
            {
                _shouldBlinkBlue = false;
                _shouldBlinkRed = false;
                IsBlinkingBlue = false;
                IsBlinkingRed = false;
            }

            OnPropertyChanged(nameof(IsBlinkingRed));
            OnPropertyChanged(nameof(IsBlinkingBlue));
        }

        [RelayCommand]
        public void TileClick() => TileClicked?.Invoke(this);

        [RelayCommand]
        public void TileMouseDown() => TileConstructionStart?.Invoke(this);

        [RelayCommand]
        public void TileLeave() => IsMouseOver = false;

        [RelayCommand]
        public void TileEnter() => IsMouseOver = true;
    }

}
