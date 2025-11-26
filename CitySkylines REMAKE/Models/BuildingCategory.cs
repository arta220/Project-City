using System.Collections.ObjectModel;
using Domain.Base;

namespace CitySimulatorWPF.Models
{
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
