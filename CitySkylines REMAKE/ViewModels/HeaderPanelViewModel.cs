using System.Windows;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views.dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Graphing;

namespace CitySimulatorWPF.ViewModels
{
    public partial class HeaderPanelViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        public event Action RemoveModeOn;

        [RelayCommand]
        private void SetRemoveMode()
        {
            RemoveModeOn?.Invoke();
        }

        public HeaderPanelViewModel(GraphService graphService)
        {
            _graphService = graphService;
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