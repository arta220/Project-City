using CitySimulatorWPF.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Buildings.Logistics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CitySimulatorWPF.Views.dialogs
{
    public partial class LogisticsCenterDialog : Window
    {
        private readonly LogisticsCenter _logisticsCenter;
        private readonly Action<LogisticsCenter> _onProcessLogistics;
        private readonly Action<LogisticsCenter> _onPrepareShipment;
        private readonly Action<LogisticsCenter> _onHireWorker;
        private readonly Action<LogisticsCenter> _onFireWorker;

        public LogisticsCenterDialog(
            LogisticsCenter logisticsCenter,
            Action<LogisticsCenter> onProcessLogistics,
            Action<LogisticsCenter> onPrepareShipment,
            Action<LogisticsCenter> onHireWorker,
            Action<LogisticsCenter> onFireWorker)
        {
            InitializeComponent();

            _logisticsCenter = logisticsCenter;
            _onProcessLogistics = onProcessLogistics;
            _onPrepareShipment = onPrepareShipment;
            _onHireWorker = onHireWorker;
            _onFireWorker = onFireWorker;

            DataContext = new LogisticsCenterDialogViewModel(logisticsCenter);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ProcessLogistics_Click(object sender, RoutedEventArgs e)
        {
            _onProcessLogistics?.Invoke(_logisticsCenter);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void PrepareShipment_Click(object sender, RoutedEventArgs e)
        {
            _onPrepareShipment?.Invoke(_logisticsCenter);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void OptimizeWarehouse_Click(object sender, RoutedEventArgs e)
        {
            _logisticsCenter?.ProcessLogistics();
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void AddVehicle_Click(object sender, RoutedEventArgs e)
        {
            var van = new Domain.Transports.Ground.DeliveryVan();
            _logisticsCenter?.AddVehicle(van);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void HireManager_Click(object sender, RoutedEventArgs e)
        {
            _onHireWorker?.Invoke(_logisticsCenter);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void HireDriver_Click(object sender, RoutedEventArgs e)
        {
            _onHireWorker?.Invoke(_logisticsCenter);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void HireWarehouseWorker_Click(object sender, RoutedEventArgs e)
        {
            _onHireWorker?.Invoke(_logisticsCenter);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void FireWorker_Click(object sender, RoutedEventArgs e)
        {
            _onFireWorker?.Invoke(_logisticsCenter);
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ((LogisticsCenterDialogViewModel)DataContext).Refresh();
        }
    }

    public class LogisticsCenterDialogViewModel : ObservableObject
    {
        private readonly LogisticsCenter _logisticsCenter;

        public LogisticsCenter LogisticsCenter => _logisticsCenter;

        // Делаем свойства изменяемыми
        public int CurrentWarehouseLoad => _logisticsCenter.CurrentWarehouseLoad;
        public int WarehouseCapacity => _logisticsCenter.WarehouseCapacity;
        public int LogisticsEfficiency => _logisticsCenter.LogisticsEfficiency;
        public int VehicleCount => _logisticsCenter.AvailableVehicles.Count;
        public int ActiveOrdersCount => _logisticsCenter.ActiveOrders.Values.Sum(list => list.Count);

        public ObservableCollection<ProductItem> WarehouseStock { get; } = new();

        public string WorkerInfo =>
            $"Сотрудников: {_logisticsCenter.GetWorkerCount()}/{_logisticsCenter.MaxOccupancy}\n" +
            $"Вакансии: {GetVacanciesInfo()}";

        public LogisticsCenterDialogViewModel(LogisticsCenter logisticsCenter)
        {
            _logisticsCenter = logisticsCenter;
            Refresh();
        }

        public void Refresh()
        {
            // Обновляем складские запасы
            WarehouseStock.Clear();
            foreach (var stock in _logisticsCenter.WarehouseStock)
            {
                if (stock.Value > 0)
                {
                    WarehouseStock.Add(new ProductItem(stock.Key.ToString(), stock.Value));
                }
            }

            // Уведомляем об изменении всех свойств
            OnPropertyChanged(nameof(CurrentWarehouseLoad));
            OnPropertyChanged(nameof(WarehouseCapacity));
            OnPropertyChanged(nameof(LogisticsEfficiency));
            OnPropertyChanged(nameof(VehicleCount));
            OnPropertyChanged(nameof(ActiveOrdersCount));
            OnPropertyChanged(nameof(WorkerInfo));
            OnPropertyChanged(nameof(WarehouseStock));
        }

        private string GetVacanciesInfo()
        {
            var vacancies = _logisticsCenter.Vacancies
                .Where(v => v.Value > 0)
                .Select(v => $"{v.Key}: {v.Value}")
                .ToList();

            return vacancies.Any() ? string.Join(", ", vacancies) : "нет вакансий";
        }
    }
}