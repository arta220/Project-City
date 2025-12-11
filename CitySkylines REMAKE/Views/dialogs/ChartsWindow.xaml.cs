using CitySimulatorWPF.ViewModels;
using System.Windows;

namespace CitySimulatorWPF.Views.dialogs
{
    public partial class ChartsWindow : Window
    {
        public ChartsWindow()
        {
            InitializeComponent();
        }

        // Конструктор с ViewModel
        public ChartsWindow(ChartsWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}