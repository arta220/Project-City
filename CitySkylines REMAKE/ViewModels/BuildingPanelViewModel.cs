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
        private MapObjectVM _selectedBuilding;

        public event Action<MapObjectVM> BuildingSelected;
        public ObservableCollection<BuildingCategory> Categories { get; private set; } = BuildingRegistry.Categories;

        [RelayCommand]
        private void SelectCategory(string categoryName)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Name == categoryName);
        }

        [RelayCommand]
        private void SelectBuilding(MapObjectVM building)
        {
            SelectedBuilding = building;
            BuildingSelected?.Invoke(SelectedBuilding);
        }
        public BuildingPanelViewModel()
        {
            if (Categories.Any())
                SelectedCategory = Categories.First();
        }
    }
}