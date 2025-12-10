using System.Windows;
using CitySimulatorWPF.ViewModels;

namespace CitySimulatorWPF.Views.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SellMaterialsDialog.xaml
    /// </summary>
    public partial class SellMaterialsDialog : Window
    {
        public SellMaterialsDialog(SellMaterialsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
