using System.Windows.Controls;
using CitySimulatorWPF.ViewModels;

namespace CitySimulatorWPF.Views.components;

public partial class BuildingPanel : UserControl
{
    public BuildingPanel()
    {
        InitializeComponent();
        // DataContext устанавливается из MainVM через привязку в MainWindow.xaml
        // Не создаем новый экземпляр здесь, иначе события не будут работать!
    }
}