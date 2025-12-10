using Domain.Buildings;
using Domain.Citizens;
using Domain.Citizens.States;
using Domain.Common.Enums;
using System.Collections.ObjectModel;
using System.Windows;

namespace CitySimulatorWPF.Views.dialogs
{
    /// <summary>
    /// Логика взаимодействия для IndustrialBuildingInfoDialog.xaml
    /// </summary>
    public partial class IndustrialBuildingInfoDialog : Window
    {
        private readonly IndustrialBuilding _building;
        private readonly Action<IndustrialBuilding> _onHireWorker;
        private readonly Action<IndustrialBuilding> _onFireWorker;

        public IndustrialBuildingInfoDialog(
            IndustrialBuilding building,
            Action<IndustrialBuilding> onHireWorker,
            Action<IndustrialBuilding> onFireWorker)
        {
            InitializeComponent();

            _building = building;
            _onHireWorker = onHireWorker;
            _onFireWorker = onFireWorker;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Заголовок
            TitleTextBlock.Text = $"Завод: {ResourceLocalization.GetLocalizedName(_building.Type)}";

            // Эффективность
            int workerCount = _building.GetWorkerCount();
            double efficiency = _building.MaxOccupancy > 0 ? (double)workerCount / _building.MaxOccupancy * 100 : 0;
            EfficiencyTextBlock.Text = $"Эффективность производства: {efficiency:F1}%";
            EfficiencyProgressBar.Value = efficiency;
            WorkerInfoTextBlock.Text = $"Рабочих: {workerCount}/{_building.MaxOccupancy}";

            // Материалы
            MaterialsListBox.Items.Clear();
            foreach (var material in _building.MaterialsBank.OrderByDescending(m => m.Value))
            {
                if (material.Value > 0)
                {
                    string localizedName = ResourceLocalization.GetLocalizedName(material.Key);
                    MaterialsListBox.Items.Add($"{localizedName}: {material.Value}");
                }
            }
            if (!MaterialsListBox.HasItems)
            {
                MaterialsListBox.Items.Add("Нет материалов");
            }

            // Продукция
            ProductsListBox.Items.Clear();
            foreach (var product in _building.ProductsBank.OrderByDescending(p => p.Value))
            {
                if (product.Value > 0)
                {
                    string localizedName = ResourceLocalization.GetLocalizedName(product.Key);
                    ProductsListBox.Items.Add($"{localizedName}: {product.Value}");
                }
            }
            if (!ProductsListBox.HasItems)
            {
                ProductsListBox.Items.Add("Нет продукции");
            }

            // Цеха
            WorkshopsListBox.Items.Clear();
            foreach (var workshop in _building.Workshops)
            {
                string inputName = ResourceLocalization.GetLocalizedName(workshop.InputMaterial);
                string outputName = ResourceLocalization.GetLocalizedName(workshop.OutputProduct);
                WorkshopsListBox.Items.Add($"{inputName} → {outputName} (коэфф: {workshop.ProductionCoefficient})");
            }

            // Кнопки
            HireWorkerButton.IsEnabled = _building.HasVacancy(CitizenProfession.FactoryWorker);
            FireWorkerButton.IsEnabled = workerCount > 0;
        }

        private void HireWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            _onHireWorker(_building);
            UpdateDisplay();
        }

        private void FireWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            _onFireWorker(_building);
            UpdateDisplay();
        }
    }
}
