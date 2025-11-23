using System.Collections.ObjectModel;

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
            // Пример данных
            var residentialCategory = new BuildingCategory { Name = "Жилые" };
            residentialCategory.Buildings.Add(new Building { Name = "Маленький дом", IconPath = "/Resources/Icons/small_house.png" });
            residentialCategory.Buildings.Add(new Building { Name = "Многоквартирный дом", IconPath = "/Resources/Icons/apartment.png" });

            var commercialCategory = new BuildingCategory { Name = "Коммерческие" };
            commercialCategory.Buildings.Add(new Building { Name = "Магазин", IconPath = "/Resources/Icons/shop.png" });
            commercialCategory.Buildings.Add(new Building { Name = "Офис", IconPath = "/Resources/Icons/office.png" });

            var industrialCategory = new BuildingCategory { Name = "Промышленные" };
            industrialCategory.Buildings.Add(new Building { Name = "Завод", IconPath = "/Resources/Icons/factory.png" });
            industrialCategory.Buildings.Add(new Building { Name = "Склад", IconPath = "/Resources/Icons/warehouse.png" });

            Categories.Add(residentialCategory);
            Categories.Add(commercialCategory);
            Categories.Add(industrialCategory);
        }
    }
}
