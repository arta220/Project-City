using System.Collections.ObjectModel;
using CitySimulatorWPF.ViewModels;

namespace CitySimulatorWPF.Models
{
    /// <summary>
    /// Категория зданий/объектов для панели строительства.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Группирует объекты для UI по логическим категориям.
    /// - Содержит коллекцию ObjectVM для визуального представления зданий.
    /// </remarks>
    public class BuildingCategory
    {
        public string Name { get; set; }
        public ObservableCollection<ObjectVM> Objects { get; set; }

        public BuildingCategory()
        {
            Objects = new ObservableCollection<ObjectVM>();
        }
    }
}
