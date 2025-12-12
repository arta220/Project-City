using CitySimulatorWPF.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Buildings.Industrial;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CitySimulatorWPF.Views.dialogs
{
    public partial class ChemicalPlantDialog : Window
    {
        private readonly ChemicalPlant _chemicalPlant;
        private readonly Action<ChemicalPlant> _onRunProduction;
        private readonly Action<ChemicalPlant> _onUpgradeTechnology;
        private readonly Action<ChemicalPlant> _onHireWorker;
        private readonly Action<ChemicalPlant> _onFireWorker;
        
        public ChemicalPlantDialog(
            ChemicalPlant chemicalPlant,
            Action<ChemicalPlant> onRunProduction,
            Action<ChemicalPlant> onUpgradeTechnology,
            Action<ChemicalPlant> onHireWorker,
            Action<ChemicalPlant> onFireWorker)
        {
            InitializeComponent();
            
            _chemicalPlant = chemicalPlant;
            _onRunProduction = onRunProduction;
            _onUpgradeTechnology = onUpgradeTechnology;
            _onHireWorker = onHireWorker;
            _onFireWorker = onFireWorker;
            
            DataContext = new ChemicalPlantDialogViewModel(chemicalPlant);
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void RunProduction_Click(object sender, RoutedEventArgs e)
        {
            _onRunProduction?.Invoke(_chemicalPlant);
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
        
        private void UpgradeTechnology_Click(object sender, RoutedEventArgs e)
        {
            _onUpgradeTechnology?.Invoke(_chemicalPlant);
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
        
        private void ImproveSafety_Click(object sender, RoutedEventArgs e)
        {
            _chemicalPlant?.ImproveSafety();
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
        
        private void ReducePollution_Click(object sender, RoutedEventArgs e)
        {
            _chemicalPlant?.ReducePollution();
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
        
        private void HireWorker_Click(object sender, RoutedEventArgs e)
        {
            _onHireWorker?.Invoke(_chemicalPlant);
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
        
        private void FireWorker_Click(object sender, RoutedEventArgs e)
        {
            _onFireWorker?.Invoke(_chemicalPlant);
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
        
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ((ChemicalPlantDialogViewModel)DataContext).Refresh();
        }
    }
    
    public class ChemicalPlantDialogViewModel : ObservableObject
    {
        private readonly ChemicalPlant _chemicalPlant;
        
        public ChemicalPlant ChemicalPlant => _chemicalPlant;
        
        public string Specialization => _chemicalPlant.Specialization.ToString();
        public int TechnologyLevel => _chemicalPlant.TechnologyLevel;
        public string SafetyLevelWithPercent => $"{_chemicalPlant.SafetyLevel}%";
        public string PollutionLevelWithPercent => $"{_chemicalPlant.PollutionLevel}%";
        public string ProductionEfficiencyWithPercent => $"{_chemicalPlant.GetProductionEfficiency()}%";
        
        public ObservableCollection<ProductItem> MaterialsList { get; } = new();
        public ObservableCollection<ProductItem> ProductsList { get; } = new();
        
        public string WorkerInfo => 
            $"Рабочих: {_chemicalPlant.GetWorkerCount()}/{_chemicalPlant.MaxOccupancy}\n" +
            $"Вакансии: {GetVacanciesInfo()}";
        
        public ChemicalPlantDialogViewModel(ChemicalPlant chemicalPlant)
        {
            _chemicalPlant = chemicalPlant;
            Refresh();
        }
        
        public void Refresh()
        {
            // Обновляем материалы
            MaterialsList.Clear();
            foreach (var material in _chemicalPlant.MaterialsBank)
            {
                if (material.Value > 0)
                {
                    MaterialsList.Add(new ProductItem(material.Key.ToString(), material.Value));
                }
            }
            
            // Обновляем продукцию
            ProductsList.Clear();
            foreach (var product in _chemicalPlant.ProductsBank)
            {
                if (product.Value > 0)
                {
                    ProductsList.Add(new ProductItem(product.Key.ToString(), product.Value));
                }
            }
            
            OnPropertyChanged(nameof(Specialization));
            OnPropertyChanged(nameof(TechnologyLevel));
            OnPropertyChanged(nameof(SafetyLevelWithPercent));
            OnPropertyChanged(nameof(PollutionLevelWithPercent));
            OnPropertyChanged(nameof(ProductionEfficiencyWithPercent));
            OnPropertyChanged(nameof(WorkerInfo));
        }
        
        private string GetVacanciesInfo()
        {
            var vacancies = _chemicalPlant.Vacancies
                .Where(v => v.Value > 0)
                .Select(v => $"{v.Key}: {v.Value}")
                .ToList();
            
            return vacancies.Any() ? string.Join(", ", vacancies) : "нет вакансий";
        }
    }
    
    public class ProductItem
    {
        public string Name { get; }
        public int Amount { get; }
        
        public ProductItem(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}