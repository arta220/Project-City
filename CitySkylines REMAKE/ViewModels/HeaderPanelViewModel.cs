using System.Windows;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views.dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Services.Graphing;
using Services.Finance;
using Services.Materials;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class HeaderPanelViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        private readonly IFinanceService _financeService;
        private readonly IMaterialInventoryService _inventoryService;

        [ObservableProperty]
        private float _cityBudget;
        public event Action RemoveModeOn;

        [RelayCommand]
        private void SetRemoveMode()
        {
            RemoveModeOn?.Invoke();
        }

        public HeaderPanelViewModel(GraphService graphService, IFinanceService financeService, IMaterialInventoryService inventoryService)
        {
            _graphService = graphService;
            _financeService = financeService;
            _inventoryService = inventoryService;
            CityBudget = _financeService.Budget.Balance;
            _financeService.BudgetChanged += OnBudgetChanged;
        }

        private void OnBudgetChanged(float change)
        {
            CityBudget = _financeService.Budget.Balance;
        }

        [RelayCommand]
        private void ShowCharts()
        {
            var chartsWindow = new ChartsWindow(
                new ChartsWindowViewModel(_graphService));
            chartsWindow.Owner = Application.Current.MainWindow;
            chartsWindow.ShowDialog();
        }

     

        [RelayCommand]
        private void BuyMaterials()
        {
            try
            {
                var vm = new BuyMaterialsViewModel(_financeService, _inventoryService);
                var dlg = new CitySimulatorWPF.Views.dialogs.BuyMaterialsDialog(vm);
                dlg.Owner = Application.Current.MainWindow;
                dlg.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии покупки материалов:\n{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SellMaterials()
        {
            try
            {
                var vm = new SellMaterialsViewModel(_financeService, _inventoryService);
                var dlg = new CitySimulatorWPF.Views.dialogs.SellMaterialsDialog(vm);
                dlg.Owner = Application.Current.MainWindow;
                dlg.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии продажи материалов:\n{ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

    }
}