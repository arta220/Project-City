using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using Services.Graphing;
using Services.Interfaces;
using System.Collections.ObjectModel;

namespace CitySimulatorWPF.ViewModels
{
    public partial class ChartsWindowViewModel : ObservableObject
    {
        private readonly GraphService _graphService;

        [ObservableProperty]
        private PlotModel _currentPlot;

        [ObservableProperty]
        private IGraphDataProvider _selectedProvider;

        public ObservableCollection<IGraphDataProvider> AvailableGraphs { get; }

        public ChartsWindowViewModel(GraphService graphService)
        {
            _graphService = graphService;
            AvailableGraphs = new ObservableCollection<IGraphDataProvider>(
                _graphService.GetAvailableGraphs());

            if (AvailableGraphs.Any())
            {
                // Вместо прямого присваивания:
                SelectedProvider = AvailableGraphs.First();

                // Принудительно создаем график
                OnSelectedProviderChanged(AvailableGraphs.First());
            }
        }

        partial void OnSelectedProviderChanged(IGraphDataProvider value)
        {
            if (value != null)
            {
                CurrentPlot = value.CreatePlotModel();
            }
        }
    }
}