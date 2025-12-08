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
        [ObservableProperty] private bool _isPreviewTile = false;
        [ObservableProperty] private bool _isMouseOver = false;

        private DispatcherTimer _blinkTimer;

        public bool HasObject => TileModel.MapObject != null;
        public bool CanBuild => !HasObject;
        public TerrainType TerrainType
        {
            get => TileModel.Terrain;
            set
            {
                if (TileModel.Terrain != value)
                {
                    TileModel.Terrain = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Тип природного ресурса на клетке.
        /// </summary>
        public NaturalResourceType ResourceType
        {
            get => TileModel.ResourceType;
            set
            {
                if (TileModel.ResourceType != value)
                {
                    TileModel.ResourceType = value;
                    OnPropertyChanged();              // ResourceType
                    OnPropertyChanged(nameof(HasResource)); // чтобы UI обновил наличие ресурса
                }
            }
        }

        /// <summary>
        /// Есть ли на клетке природный ресурс.
        /// </summary>
        public bool HasResource => TileModel.ResourceType != NaturalResourceType.None && TileModel.ResourceAmount > 0;

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
                if (IsBlinkingRed)
                    OnPropertyChanged(nameof(IsBlinkingRed));
            };
            _blinkTimer.Start();

            TileModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TileModel.MapObject))
                {
                    OnPropertyChanged(nameof(HasObject));
                    UpdateBlinkingState();

                    if (TileModel.MapObject is ResidentialBuilding residential)
                    {
                        residential.Utilities.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(residential.Utilities.HasBrokenUtilities))
                                UpdateBlinkingState();
                        };
                    }
                }
                // добавляем реакцию на ресурсы
                else if (e.PropertyName == nameof(TileModel.ResourceAmount) ||
                         e.PropertyName == nameof(TileModel.ResourceType))
                {
                    OnPropertyChanged(nameof(ResourceAmount));
                    OnPropertyChanged(nameof(ResourceType));
                    OnPropertyChanged(nameof(HasResource));
                }
            };

            UpdateBlinkingState();
        }

        public void UpdateBlinkingState()
        {
            if (TileModel.MapObject is ResidentialBuilding residential)
                IsBlinkingRed = residential.Utilities.HasBrokenUtilities;
            else
                IsBlinkingRed = false;

            OnPropertyChanged(nameof(IsBlinkingRed));
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
