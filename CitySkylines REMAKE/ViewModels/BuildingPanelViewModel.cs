using CitySimulatorWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CitySimulatorWPF.ViewModels
{
    public class BuildingPanelViewModel : ViewModelBase
    {
        private BuildingCategory _selectedCategory;

        public ObservableCollection<BuildingCategory> Categories { get; private set; }

        public BuildingCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        public ICommand SelectCategoryCommand { get; }

        public BuildingPanelViewModel()
        {
            Categories = BuildingRegistry.Categories;
            SelectCategoryCommand = new RelayCommand(SelectCategory);
            // Устанавливаем первую категорию как выбранную по умолчанию
            if (Categories.Any())
            {
                SelectedCategory = Categories.First();
            }
        }

        private void SelectCategory(object categoryName)
        {
            SelectedCategory = Categories.FirstOrDefault(c => c.Name == (string)categoryName);
        }
    }

    // Простая реализация ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
