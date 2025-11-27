using CommunityToolkit.Mvvm.ComponentModel;
using CitySkylines_REMAKE.Models.Enums;

namespace CitySimulatorWPF.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        public BuildingPanelViewModel BuildingPanelVM { get; }
        public MapVM MapVM { get; }

        public MainVM(MapVM mapVM, BuildingPanelViewModel buildingPanelVM)
        {
            MapVM = mapVM;
            BuildingPanelVM = buildingPanelVM;

            BuildingPanelVM.BuildingSelected += building =>
            {
                MapVM.SelectedObject = building;
                MapVM.CurrentMode = MapInteractionMode.Build;
            };

            BuildingPanelVM.RemoveModeChanged += (isActive) =>
            {
                if (isActive)
                {
                    MapVM.ActivateRemoveMode();
                }
                else
                {
                    MapVM.CurrentMode = MapInteractionMode.None;
                }
            };
        }
    }
}