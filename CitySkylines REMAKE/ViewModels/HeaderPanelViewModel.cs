using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views.dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.CommercialVisits;
using Services.Graphing;
using System.Windows;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class HeaderPanelViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        private readonly ICommercialVisitService _commercialVisitService; // добавил сервис для коммерции (он собирает данные о посещениях коммерческих зданий)
        public event Action RemoveModeOn;

        [RelayCommand]
        private void SetRemoveMode()
        {
            RemoveModeOn?.Invoke();
        }

        public HeaderPanelViewModel(GraphService graphService, ICommercialVisitService commercialVisitService)
        {
            _graphService = graphService;
            _commercialVisitService = commercialVisitService; // добавил сервис для коммерции (он собирает данные о посещениях коммерческих зданий)
        }

        [RelayCommand]
        private void ShowCharts()
        {
            var chartsWindow = new ChartsWindow(
                new ChartsWindowViewModel(_graphService, _commercialVisitService));
            chartsWindow.Owner = Application.Current.MainWindow;
            chartsWindow.ShowDialog();
        }

    }
}