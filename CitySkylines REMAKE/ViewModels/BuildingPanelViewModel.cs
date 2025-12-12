using CitySimulatorWPF.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel панели строительства зданий.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Управляет выбором категории зданий и конкретного объекта для строительства.
    /// - Инкапсулирует текущую выбранную категорию (<see cref="SelectedCategory"/>) и объект (<see cref="SelectedObject"/>).
    /// - Генерирует событие <see cref="BuildingSelected"/> для уведомления других компонентов (например, MapVM) о выбранном объекте.
    ///
    /// Контекст использования:
    /// - Используется в UI для панели строительства в CitySimulatorWPF.
    /// - Взаимодействует с MapVM: при выборе объекта MapVM переключается в режим строительства.
    ///
    /// Расширяемость:
    /// - Можно добавить фильтры по стоимости, уровню игрока или доступности зданий.
    /// - Можно добавить сортировку и иконки категорий.
    /// </remarks>
    public partial class BuildingPanelViewModel : ObservableObject
    {
        /// <summary>
        /// Текущая выбранная категория зданий.
        /// </summary>
        [ObservableProperty]
        private BuildingCategory _selectedCategory;

        /// <summary>
        /// Текущий выбранный объект для строительства.
        /// </summary>
        [ObservableProperty]
        private ObjectVM _selectedObject;

        /// <summary>
        /// Событие вызывается при выборе здания пользователем.
        /// </summary>
        public event Action<ObjectVM> BuildingSelected;

        /// <summary>
        /// Событие для изменения режима удаления
        /// </summary>
        public event Action<bool> RemoveModeChanged;

        /// <summary>
        /// Коллекция категорий зданий для панели.
        /// </summary>
        public ObservableCollection<BuildingCategory> Categories { get; private set; } = BuildingRegistry.Categories;

        /// <summary>
        /// Команда выбора категории зданий.
        /// </summary>
        /// <param name="categoryName">Имя категории.</param>
        [RelayCommand]
        private void SelectCategory(string categoryName)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Name == categoryName);
        }

        /// <summary>
        /// Команда выбора конкретного здания.
        /// </summary>
        /// <param name="@object">Объект здания.</param>
        private static void LogToFile(string message)
        {
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CitySimulator", "debug.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
            }
            catch { }
        }

        [RelayCommand]
        private void SelectBuilding(ObjectVM @object)
        {
            try
            {
                var msg = $"[SelectBuilding] Called with: {@object?.Factory?.GetType().Name ?? "null"}, Factory: {@object?.Factory?.GetType().FullName ?? "null"}";
                System.Diagnostics.Debug.WriteLine(msg);
                LogToFile(msg);
                
                if (@object == null)
                {
                    var nullMsg = "[SelectBuilding] Received null object!";
                    System.Diagnostics.Debug.WriteLine(nullMsg);
                    LogToFile(nullMsg);
                    return;
                }
                
                SelectedObject = @object;
                var msg2 = $"[SelectBuilding] SelectedObject set to: {SelectedObject?.Factory?.GetType().Name ?? "null"}";
                System.Diagnostics.Debug.WriteLine(msg2);
                LogToFile(msg2);
                
                var hasSubscribers = BuildingSelected != null;
                var msg3 = $"[SelectBuilding] BuildingSelected event has subscribers: {hasSubscribers}";
                System.Diagnostics.Debug.WriteLine(msg3);
                LogToFile(msg3);
                
                BuildingSelected?.Invoke(SelectedObject);
                var msg4 = $"[SelectBuilding] BuildingSelected event invoked";
                System.Diagnostics.Debug.WriteLine(msg4);
                LogToFile(msg4);
            }
            catch (Exception ex)
            {
                var errorMsg = $"[SelectBuilding] ERROR: {ex.Message}\n{ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(errorMsg);
                LogToFile(errorMsg);
            }
        }

        /// <summary>
        /// Создаёт экземпляр панели строительства и устанавливает первую категорию по умолчанию.
        /// </summary>
        public BuildingPanelViewModel()
        {
            if (Categories.Any())
                SelectedCategory = Categories.First();
        }

    }
}
