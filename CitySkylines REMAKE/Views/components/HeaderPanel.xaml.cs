using System.Windows;
using System.Windows.Controls;
using CitySkylines_REMAKE;
using CitySimulatorWPF.ViewModels;
using Domain.Map.Generation;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System.Linq;
using Domain.Common.Base;

namespace CitySimulatorWPF.Views.components
{
    /// <summary>
    /// Логика взаимодействия для Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        // ТВОЙ СТАРЫЙ МЕТОД - НЕ ТРОГАЕМ
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Твой код для графиков
        }

        // Кнопка сохранения
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();
                var saveManager = app?._serviceProvider?.GetService<GameSaveManager>();

                if (simulation == null || saveManager == null)
                {
                    MessageBox.Show("Не могу получить доступ к сервисам", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем текущее состояние
                saveManager.SaveCurrentState(simulation.MapModel);
                MessageBox.Show("💾 Игра сохранена!", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка загрузки
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();
                var saveManager = app?._serviceProvider?.GetService<GameSaveManager>();
                var mapVM = app?._serviceProvider?.GetService<MapVM>();

                if (simulation == null || saveManager == null)
                {
                    MessageBox.Show("Не могу получить доступ к сервисам", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!saveManager.HasSavedGame())
                {
                    MessageBox.Show("Нет сохраненной игры", "Загрузка",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Загружаем сохраненное состояние
                saveManager.LoadSavedState(simulation.MapModel);

                // Обновляем иконки зданий в MapVM
                if (mapVM != null)
                {
                    // Очищаем все иконки
                    mapVM.BuildingIcons.Clear();

                    // Добавляем иконки для всех объектов на карте
                    for (int x = 0; x < simulation.MapModel.Width; x++)
                    {
                        for (int y = 0; y < simulation.MapModel.Height; y++)
                        {
                            var mapObject = simulation.MapModel[x, y].MapObject;
                            if (mapObject != null)
                            {
                                var (placement, found) = simulation.GetMapObjectPlacement(mapObject);
                                if (found && placement != null)
                                {
                                    const int tileSize = 20;
                                    var iconVm = new BuildingIconVM(mapObject, (Placement)placement, tileSize);
                                    mapVM.BuildingIcons.Add(iconVm);
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("📂 Игра загружена!", "Загрузка",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка очистки карты
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Спрашиваем подтверждение
                var result = MessageBox.Show(
                    "УДАЛИТЬ ВСЕ ЗДАНИЯ С КАРТЫ?\n\nЭто действие нельзя отменить!",
                    "Очистка карты",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                // Получаем доступ к симуляции через App
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();

                if (simulation == null)
                {
                    MessageBox.Show("Не могу получить доступ к симуляции");
                    return;
                }

                // Собираем все объекты на карте
                var objectsToRemove = new List<MapObject>();
                for (int x = 0; x < simulation.MapModel.Width; x++)
                {
                    for (int y = 0; y < simulation.MapModel.Height; y++)
                    {
                        var mapObject = simulation.MapModel[x, y].MapObject;
                        if (mapObject != null && !objectsToRemove.Contains(mapObject))
                        {
                            objectsToRemove.Add(mapObject);
                        }
                    }
                }

                // Удаляем все объекты используя TryRemove (это вызовет события и удалит иконки)
                int removedCount = 0;
                foreach (var mapObject in objectsToRemove)
                {
                    if (simulation.TryRemove(mapObject))
                    {
                        removedCount++;
                    }
                }

                // Также удаляем всех граждан и машины через сервисы
                var citizenService = app?._serviceProvider?.GetService<Services.CitizensSimulation.CitizenSimulationService>();
                var transportService = app?._serviceProvider?.GetService<Services.TransportSimulation.TransportSimulationService>();

                int citizensRemoved = 0;
                int transportsRemoved = 0;

                if (citizenService != null)
                {
                    var citizens = citizenService.Citizens.ToList();
                    foreach (var citizen in citizens)
                    {
                        simulation.RemoveCitizen(citizen);
                        citizensRemoved++;
                    }
                }

                if (transportService != null)
                {
                    var transports = transportService.Transports.ToList();
                    foreach (var transport in transports)
                    {
                        simulation.RemoveTransport(transport);
                        transportsRemoved++;
                    }
                }

                MessageBox.Show($"✅ Карта очищена!\nУдалено объектов: {removedCount}\nУдалено граждан: {citizensRemoved}\nУдалено машин: {transportsRemoved}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}