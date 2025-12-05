using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;
using Domain.Transports.Ground;
using Domain.Transports.States;

namespace CitySimulatorWPF.ViewModels
{
    public partial class PersonalCarVM : ObservableObject
    {
        public Transport Car { get; }

        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        [ObservableProperty]
        private int _pixelX;

        [ObservableProperty]
        private int _pixelY;

        [ObservableProperty]
        private string _carColor;

        public PersonalCarVM(Transport car)
        {
            Car = car;
            UpdatePosition();
            UpdateColor();
        }

        public void UpdatePosition()
        {
            X = Car.Position.X;
            Y = Car.Position.Y;
            PixelX = X * 20;
            PixelY = Y * 20;

            UpdateColor();
        }

        private void UpdateColor()
        {
            // Простая подсветка машины:
            // - едет (в любую сторону): красная
            // - стоит (дома или на работе): серая
            if (Car.State == TransportState.DrivingToWork ||
                Car.State == TransportState.DrivingHome)
            {
                CarColor = "Red";
            }
            else
            {
                CarColor = "Gray";
            }
        }
    }
}
