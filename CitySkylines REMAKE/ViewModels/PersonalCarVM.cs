using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;
using Domain.Transports.Ground;

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

        public PersonalCarVM(Transport car)
        {
            Car = car;
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            X = Car.Position.X;
            Y = Car.Position.Y;
            PixelX = X * 20;
            PixelY = Y * 20;
        }
    }
}
