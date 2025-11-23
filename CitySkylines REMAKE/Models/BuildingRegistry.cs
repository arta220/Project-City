using System.Collections.ObjectModel;
using Domain.Buildings;

namespace CitySimulatorWPF.Models
{
    public static class BuildingRegistry
    {
        public static ObservableCollection<BuildingCategory> Categories { get; private set; }

        static BuildingRegistry()
        {
            Categories = new ObservableCollection<BuildingCategory>();
            Initialize();
        }

        private static void Initialize()
        {
            var residentialCategory = new BuildingCategory { Name = "Жилые" };
            residentialCategory.Buildings.Add(new ResidentialBuilding("Маленький дом", "", width: 2, height: 2));
            residentialCategory.Buildings.Add(new ResidentialBuilding("Многоквартирный дом", "", floors: 5, width: 4, height: 4));

            var commercialCategory = new BuildingCategory { Name = "Коммерческие" };
            commercialCategory.Buildings.Add(new CommercialBuilding("Магазин", "", width: 3, height: 2));
            commercialCategory.Buildings.Add(new CommercialBuilding("Офис", "", floors: 10, width: 3, height: 3));

            var industrialCategory = new BuildingCategory { Name = "Промышленные" };
            industrialCategory.Buildings.Add(new IndustrialBuilding("Завод", "", width: 5, height: 5));
            industrialCategory.Buildings.Add(new IndustrialBuilding("Склад", "", width: 4, height: 6));

            Categories.Add(residentialCategory);
            Categories.Add(commercialCategory);
            Categories.Add(industrialCategory);
        }
    }
}
