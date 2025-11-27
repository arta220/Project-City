using CitySimulatorWPF.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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
        
        [ObservableProperty]
        private bool _isRemoveModeActive; // Smirnov*

        /// <summary>
        /// Событие вызывается при выборе здания пользователем.
        /// </summary>
        public event Action<ObjectVM> BuildingSelected;

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
            IsRemoveModeActive = false;
        }

        /// <summary>
        /// Команда выбора конкретного здания.
        /// </summary>
        /// <param name="@object">Объект здания.</param>
        [RelayCommand]
        private void SelectBuilding(ObjectVM @object)
        {
            SelectedObject = @object;
            BuildingSelected?.Invoke(SelectedObject);
            IsRemoveModeActive = false;
        }

        /// <summary>
        /// Создаёт экземпляр панели строительства и устанавливает первую категорию по умолчанию.
        /// </summary>
        public BuildingPanelViewModel()
        {
            if (Categories.Any())
                SelectedCategory = Categories.First();
        }

        // Smirnov*
        [RelayCommand]
        private void ToggleRemoveMode()
        {
            IsRemoveModeActive = !IsRemoveModeActive; 
            RemoveModeChanged?.Invoke(IsRemoveModeActive);
        }
    }
}
