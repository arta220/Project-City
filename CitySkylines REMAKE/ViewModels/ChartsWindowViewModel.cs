using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using Services.CommercialVisits;
using Services.Graphing;
using Services.Interfaces;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace CitySimulatorWPF.ViewModels
{
    public partial class ChartsWindowViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        private readonly ICommercialVisitService? _commercialVisitService; // добавил сервис для коммерции (он собирает данные о посещениях коммерческих зданий)

        [ObservableProperty]
        private PlotModel _currentPlot;

        [ObservableProperty]
        private IGraphDataProvider _selectedProvider;

        public ObservableCollection<IGraphDataProvider> AvailableGraphs { get; }

        public ChartsWindowViewModel(
            GraphService graphService,
            ICommercialVisitService? commercialVisitService = null)
        {
            _graphService = graphService;
            _commercialVisitService = commercialVisitService; // добавил сервис для коммерции (он собирает данные о посещениях коммерческих зданий)
            AvailableGraphs = new ObservableCollection<IGraphDataProvider>(
                _graphService.GetAvailableGraphs());
            
            // добавил сервис для коммерции (он собирает данные о посещениях коммерческих зданий)
            if (_commercialVisitService != null)
                _commercialVisitService.StatisticsUpdated += OnStatisticsUpdated;

            if (AvailableGraphs.Any())
            {
                SelectedProvider = AvailableGraphs.First();
            }
        }

        partial void OnSelectedProviderChanged(IGraphDataProvider value)
        {
            if (value != null)
            {
                CurrentPlot = value.CreatePlotModel();
            }
        }

        private void OnStatisticsUpdated()
        {
            if (SelectedProvider?.SystemName != "Посещения коммерции")
                return;

            void Refresh() => CurrentPlot = SelectedProvider.CreatePlotModel();

            var dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            if (dispatcher.CheckAccess())
                Refresh();
            else
                dispatcher.Invoke(Refresh);
        }
    }
}