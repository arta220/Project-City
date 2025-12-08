using System.Windows;
using CitySimulatorWPF.ViewModels;

namespace CitySimulatorWPF.Views.dialogs
{
    /// <summary>
    /// Логика взаимодействия для BuyMaterialsDialog.xaml
    /// </summary>
    public partial class BuyMaterialsDialog : Window
    {
        public BuyMaterialsDialog(BuyMaterialsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
