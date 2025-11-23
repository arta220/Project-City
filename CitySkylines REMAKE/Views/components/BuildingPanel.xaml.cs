using System.Windows.Controls;
using CitySimulatorWPF.ViewModels;

namespace CitySimulatorWPF.Views.components;

public partial class BuildingPanel : UserControl
{
    public BuildingPanel()
    {
        InitializeComponent();
        DataContext = new BuildingPanelViewModel();
    }
}