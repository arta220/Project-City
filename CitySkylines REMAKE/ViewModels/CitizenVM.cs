using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Citizens;
using Domain.Citizens.States;
using System.ComponentModel;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для представления жителя на карте города.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Инкапсулирует состояние жителя (<see cref="Citizen"/>).
    /// - Отслеживает изменения позиции жителя и обновляет координаты для визуализации.
    /// - Преобразует координаты в пиксели для UI (<see cref="_pixelX"/> и <see cref="_pixelY"/>).
    ///
    /// Контекст использования:
    /// - Привязан к визуальным элементам карты для отображения жителей.
    /// - Используется в коллекции <see cref="MapVM.Citizens"/> для биндинга ObservableCollection.
    ///
    /// Расширяемость:
    /// - Можно добавить визуальные свойства (цвет, иконка, анимация состояния).
    /// - Можно расширить логику отслеживания других состояний жителя (например, настроение, здоровье).
    /// </remarks>
    public partial class CitizenVM : ObservableObject
    {
        /// <summary>
        /// Модель жителя, инкапсулируемая ViewModel.
        /// </summary>
        public Citizen Citizen { get; }

        /// <summary>
        /// X-позиция жителя на карте в клетках.
        /// </summary>
        [ObservableProperty]
        private int _x;

        /// <summary>
        /// Y-позиция жителя на карте в клетках.
        /// </summary>
        [ObservableProperty]
        private int _y;

        /// <summary>
        /// Цвет жителя для визуального отображения.
        /// </summary>
        [ObservableProperty]
        private string _citizenColor;

        /// <summary>
        /// X-позиция жителя в пикселях для UI.
        /// </summary>
        [ObservableProperty]
        public int _pixelX;

        /// <summary>
        /// Y-позиция жителя в пикселях для UI.
        /// </summary>
        [ObservableProperty]
        public int _pixelY;

        /// <summary>
        /// Создаёт новый экземпляр CitizenVM и подписывается на изменения позиции жителя.
        /// </summary>
        /// <param name="citizen">Модель жителя.</param>
        public CitizenVM(Citizen citizen)
        {
            Citizen = citizen;
            UpdatePosition();

            Citizen.PropertyChanged += OnCitizenPropertyChanged;
        }

        /// <summary>
        /// Обрабатывает событие изменения свойств модели жителя.
        /// </summary>
        private void OnCitizenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Citizen.Position))
            {
                UpdatePosition();
            }
        }

        /// <summary>
        /// Обновляет координаты жителя для отображения на карте и в пикселях.
        /// </summary>
        public void UpdatePosition()
        {
            X = Citizen.Position.X;
            Y = Citizen.Position.Y;
            PixelX = X * 20;
            PixelY = Y * 20;
        }
    }
}
