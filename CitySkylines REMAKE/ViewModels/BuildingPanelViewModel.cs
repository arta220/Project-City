using CitySimulatorWPF.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CitySimulatorWPF.ViewModels
{
    public partial class BuildingPanelViewModel : ObservableObject
    {
        [ObservableProperty]
        private BuildingCategory _selectedCategory;

        public ObservableCollection<BuildingCategory> Categories { get; private set; } = BuildingRegistry.Categories;

        [RelayCommand]
        private void SelectCategory(string categoryName)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Name == categoryName);
        }

        public BuildingPanelViewModel()
        {
            if (Categories.Any())
                SelectedCategory = Categories.First();
        }
    }
}