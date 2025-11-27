using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using Domain.Enums;
using Domain.Base;

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
        /// <summary>
        /// Событие, вызываемое при клике на клетку.
        /// </summary>
        public event Action<TileVM> TileClicked;

        /// <summary>
        /// Событие, вызываемое при начале строительства на клетке.
        /// </summary>
        public event Action<TileVM> TileConstructionStart;

        /// <summary>
        /// Модель клетки карты.
        /// </summary>
        public TileModel TileModel { get; }

        /// <summary>
        /// Координата X клетки.
        /// </summary>
        [ObservableProperty]
        public int _x;

        /// <summary>
        /// Координата Y клетки.
        /// </summary>
        [ObservableProperty]
        public int _y;

        /// <summary>
        /// Флаг, показывающий, является ли клетка превью для строительства.
        /// </summary>
        [ObservableProperty]
        private bool _isPreviewTile = false;

        /// <summary>
        /// Флаг наведения мыши на клетку.
        /// </summary>
        [ObservableProperty]
        private bool _isMouseOver = false;

        /// <summary>
        /// Есть ли на клетке объект.
        /// </summary>
        public bool HasObject => TileModel.MapObject != null;

        /// <summary>
        /// Можно ли построить объект на этой клетке.
        /// </summary>
        public bool CanBuild => !HasObject;

        /// <summary>
        /// Тип местности клетки.
        /// </summary>
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

        /// <summary>
        /// Создаёт ViewModel для клетки карты.
        /// </summary>
        /// <param name="tileModel">Модель клетки</param>
        public TileVM(TileModel tileModel)
        {
            TileModel = tileModel;
            X = tileModel.Position.X;
            Y = tileModel.Position.Y;

            // Подписка на изменения объекта, чтобы обновлять свойства UI
            TileModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TileModel.MapObject))
                    OnPropertyChanged(nameof(HasObject));
            };
        }

        /// <summary>
        /// Команда клика по клетке.
        /// </summary>
        [RelayCommand]
        public void TileClick() => TileClicked?.Invoke(this);

        /// <summary>
        /// Команда нажатия мыши на клетке (начало строительства).
        /// </summary>
        [RelayCommand]
        public void TileMouseDown() => TileConstructionStart?.Invoke(this);

        /// <summary>
        /// Команда ухода мыши с клетки.
        /// </summary>
        [RelayCommand]
        public void TileLeave() => IsMouseOver = false;

        /// <summary>
        /// Команда наведения мыши на клетку.
        /// </summary>
        [RelayCommand]
        public void TileEnter() => IsMouseOver = true;
    }
}
