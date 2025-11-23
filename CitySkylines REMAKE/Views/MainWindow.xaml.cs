using System.Windows;
using CitySimulatorWPF.ViewModels;

namespace CitySimulatorWPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainVM vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}