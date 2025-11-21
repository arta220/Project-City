using CommunityToolkit.Mvvm.ComponentModel;

namespace CitySkylines_REMAKE.ViewModels
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