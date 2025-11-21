using CitySkylines_REMAKE.ViewModels;
using System.Windows;

namespace CitySkylines_REMAKE.Views;

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