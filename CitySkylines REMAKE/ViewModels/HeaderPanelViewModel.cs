using System.Windows;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views.dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Map;
using CitySimulatorWPF.Services;
using Services;
using Services.Graphing;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class HeaderPanelViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        private readonly MovingEntityAnimationService _movingEntityAnimationService;

        public event Action RemoveModeOn;

        public HeaderPanelViewModel(
            GraphService graphService,
            MovingEntityAnimationService movingEntityAnimationService)
        {
            _graphService = graphService;
            _movingEntityAnimationService = movingEntityAnimationService;
        }

        [RelayCommand]
        private void SetRemoveMode()
        {
            RemoveModeOn?.Invoke();
        }
        [RelayCommand]
        private void BuyMaterials()
        {
            _movingEntityAnimationService.SendPlaneToAirport();
        }

        [RelayCommand]
        private void ShowCharts()
        {
            var chartsWindow = new ChartsWindow(
                new ChartsWindowViewModel(_graphService));

            chartsWindow.Owner = Application.Current.MainWindow;
            chartsWindow.ShowDialog();
        }
    }
}