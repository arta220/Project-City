using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;
using Domain.Transports.Ground;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для личного автомобиля на карте.
    /// </summary>
    public partial class PersonalCarVM : ObservableObject
    {
        /// <summary>
        /// Модель автомобиля.
        /// </summary>
        public Transport Car { get; }

        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        [ObservableProperty]
        private int _pixelX;

        [ObservableProperty]
        private int _pixelY;

        public PersonalCarVM(Transport car)
        {
            Car = car;
            UpdatePosition();
        }

        /// <summary>
        /// Обновляет координаты автомобиля в клетках и пикселях.
        /// Вызывается менеджером машин при каждом обновлении симуляции.
        /// </summary>
        public void UpdatePosition()
        {
            X = Car.Position.X;
            Y = Car.Position.Y;
            PixelX = X * 20;
            PixelY = Y * 20;
        }
    }
}
