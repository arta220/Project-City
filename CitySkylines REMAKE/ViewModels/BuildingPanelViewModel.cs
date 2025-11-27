using CitySimulatorWPF.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace CitySimulatorWPF.ViewModels
{
    public partial class BuildingPanelViewModel : ObservableObject
    {
        [ObservableProperty]
        private BuildingCategory _selectedCategory;

        [ObservableProperty]
        private ObjectVM _selectedObject;
        
        [ObservableProperty]
        private bool _isRemoveModeActive; // Smirnov*

        public event Action<ObjectVM> BuildingSelected;
        public event Action<bool> RemoveModeChanged; // Smirnov*
        public ObservableCollection<BuildingCategory> Categories { get; private set; } = BuildingRegistry.Categories;

        [RelayCommand]
        private void SelectCategory(string categoryName)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Name == categoryName);
            IsRemoveModeActive = false;
        }

        [RelayCommand]
        private void SelectBuilding(ObjectVM @object)
        {
            SelectedObject = @object;
            BuildingSelected?.Invoke(SelectedObject);
            IsRemoveModeActive = false;
        }
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