using CommunityToolkit.Mvvm.ComponentModel;
using CitySkylines_REMAKE.Models.Enums;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;
using Services.Graphing;
using CitySimulatorWPF.Views.dialogs;
using CitySkylines_REMAKE.ViewModels;
using CitySkylines_REMAKE;
using Microsoft.Extensions.DependencyInjection;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// Основная ViewModel приложения.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Инкапсулирует основные под-VМ приложения: карту (<see cref="MapVM"/>) и панель строительства (<see cref="BuildingPanelViewModel"/>).
    /// - Управляет связью между выбором здания в панели и режимом строительства на карте.
    ///
    /// Контекст использования:
    /// - Привязан к MainWindow в WPF.
    /// - Использует DI для получения экземпляров <see cref="MapVM"/> и <see cref="BuildingPanelViewModel"/>.
    ///
    /// Расширяемость:
    /// - Можно добавить новые панели управления или дополнительные под-VМ.
    /// - Логика реакции на выбор объектов может быть расширена для других типов построек или интерактивных действий.
    /// </remarks>
    public partial class MainVM : ObservableObject
    {

        private readonly GraphService _graphService;
        /// <summary>
        /// Панель выбора зданий.
        /// </summary>
        public BuildingPanelViewModel BuildingPanelVM { get; }

        public HeaderPanelViewModel HeaderPanelVM { get; }

        /// <summary>
        /// ViewModel карты города.
        /// </summary>
        public MapVM MapVM { get; }

        /// <summary>
        /// Инициализирует MainVM с зависимостями на MapVM и панель строительства.
        /// </summary>
        /// <param name="mapVM">ViewModel карты города.</param>
        /// <param name="buildingPanelVM">ViewModel панели строительства.</param>
        public MainVM(
            MapVM mapVM, 
            BuildingPanelViewModel buildingPanelVM,
            HeaderPanelViewModel headerPanelVM,
            GraphService graphService)
        {
            _graphService = graphService;
            MapVM = mapVM;
            BuildingPanelVM = buildingPanelVM;
            HeaderPanelVM = headerPanelVM;

            // Подписка на событие выбора здания в панели
            BuildingPanelVM.BuildingSelected += building =>
            {
                MapVM.SelectedObject = building;
                MapVM.CurrentMode = MapInteractionMode.Build;
            };

            HeaderPanelVM.RemoveModeOn += () =>
            {
                MapVM.CurrentMode = MapInteractionMode.Remove;
            };
        }

        [RelayCommand]
        private void ShowCharts()
        {
            try
            {
                // Получаем App и ServiceProvider
                var app = Application.Current as App;
                if (app?._serviceProvider == null)
                {
                    MessageBox.Show("DI контейнер не инициализирован");
                    return;
                }

                // Получаем GraphService
                var graphService = app._serviceProvider.GetService<GraphService>();
                if (graphService == null)
                {
                    MessageBox.Show("GraphService не найден в DI контейнере");
                    return;
                }

                // Создаем ViewModel и окно
                var viewModel = new ChartsWindowViewModel(graphService);
                var chartsWindow = new ChartsWindow
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow // Делаем главное окно владельцем
                };

                chartsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии графиков:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
