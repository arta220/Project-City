using System.Windows;
using CitySimulatorWPF.ViewModels;
using CitySimulatorWPF.Views.dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Services;
using Services.Graphing;
using Services.SaveLoad;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class HeaderPanelViewModel : ObservableObject
    {
        private readonly GraphService _graphService;
        private readonly MapVM _mapVM;
        private readonly Simulation _simulation;
        private readonly ISaveLoadService _saveLoadService;
        public event Action RemoveModeOn;

        [RelayCommand]
        private void SetRemoveMode()
        {
            RemoveModeOn?.Invoke();
        }

        public HeaderPanelViewModel(GraphService graphService, MapVM mapVM, Simulation simulation, ISaveLoadService saveLoadService)
        {
            _graphService = graphService;
            _mapVM = mapVM;
            _simulation = simulation;
            _saveLoadService = saveLoadService;
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
        private void ClearMap()
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите очистить карту? Все объекты будут удалены.",
                "Подтверждение очистки",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                _mapVM?.ClearMap();
            }
        }

        [RelayCommand]
        private void SaveGame()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "savegame.json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                if (_saveLoadService.SaveGame(saveDialog.FileName, _simulation))
                {
                    MessageBox.Show("Игра успешно сохранена!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении игры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void LoadGame()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (openDialog.ShowDialog() == true)
            {
                if (_saveLoadService.LoadGame(openDialog.FileName, _simulation))
                {
                    // Обновляем UI после загрузки
                    _mapVM?.ClearMap();
                    MessageBox.Show("Игра успешно загружена!", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при загрузке игры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}