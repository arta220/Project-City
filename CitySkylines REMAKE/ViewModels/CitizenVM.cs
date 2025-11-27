using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Citizens;
using Domain.Citizens.States;
using System.ComponentModel;

namespace CitySimulatorWPF.ViewModels
{
    public partial class CitizenVM : ObservableObject
    {
        public Citizen Citizen { get; }

        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        [ObservableProperty]
        private string _citizenColor;

        [ObservableProperty]
        public int _pixelX;

        [ObservableProperty]
        public int _pixelY;
        public CitizenVM(Citizen citizen)
        {
            Citizen = citizen;
            UpdatePosition();
            UpdateColor();

            Citizen.PropertyChanged += OnCitizenPropertyChanged;
        }

        private void OnCitizenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Citizen.Position))
            {
                UpdatePosition();
            }
            if (e.PropertyName == nameof(Citizen.State))
            {
                UpdateColor();
            }
        }

        public void UpdatePosition()
        {
            X = Citizen.Position.X;
            Y = Citizen.Position.Y;
            PixelX = X * 20;
            PixelY = Y * 20;
        }

        private void UpdateColor()
        {
            CitizenColor = Citizen.State switch
            {
                CitizenState.GoingHome => "Red",
                CitizenState.GoingToWork => "Blue",
                CitizenState.Idle => "Green",
                _ => "Gray"
            };
        }
    }
}