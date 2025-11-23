using CommunityToolkit.Mvvm.ComponentModel;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public MapVM MapVM { get; }

        public MainVM(MapVM mapVM)
        {
            MapVM = mapVM;
        }
    }
}