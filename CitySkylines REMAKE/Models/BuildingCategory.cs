using System.Collections.ObjectModel;
using Domain.Base;

namespace CitySimulatorWPF.Models
{
    public class BuildingCategory
    {
        public string Name { get; set; }
        public ObservableCollection<MapObjectVM> Objects { get; set; }

        public BuildingCategory()
        {
            Objects = new ObservableCollection<MapObjectVM>();
        }
    }
}
