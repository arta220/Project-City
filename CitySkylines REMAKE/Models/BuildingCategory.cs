using System.Collections.ObjectModel;
using Domain.Base;

namespace CitySimulatorWPF.Models
{
    public class BuildingCategory
    {
        public string Name { get; set; }
        public ObservableCollection<BuildingVM> Buildings { get; set; }

        public BuildingCategory()
        {
            Buildings = new ObservableCollection<BuildingVM>();
        }
    }
}
