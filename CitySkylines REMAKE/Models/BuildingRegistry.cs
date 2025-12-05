using CitySimulatorWPF.ViewModels;
using Domain.Base;
using Domain.Enums;
using Domain.Factories;
using Domain.Map;
using System.Collections.ObjectModel;

namespace CitySimulatorWPF.Models
{
    /// <summary>
    /// Центральный реестр типов зданий и инфраструктуры.
    /// </summary>
    public static class BuildingRegistry
    {
        /// <summary>
        /// Список категорий объектов для строительства.
        /// </summary>
        public static ObservableCollection<BuildingCategory> Categories { get; private set; }

        static BuildingRegistry()
        {
            Categories = new ObservableCollection<BuildingCategory>();
            Initialize();
        }

        private static void Initialize()
        {
            // Жилые
            var residential = new BuildingCategory { Name = "Жилые" };

            residential.Objects.Add(new ObjectVM(
                new SmallHouseFactory(),
                "Маленький дом",
                "Assets/Icons/SmallHouse.png"
            ));

            residential.Objects.Add(new ObjectVM(
                new ApartmentFactory(),
                "Многоквартирный дом",
                "Assets/Icons/Apartment.png"
            ));

            var commercial = new BuildingCategory { Name = "Коммерческие" };
            // Коммерческие
            commercial.Objects.Add(new ObjectVM(
                new PharmacyFactory(),
                "Аптека",
                "Assets/Icons/Pharmacy.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new ShopFactory(),
                "Магазин",
                "Assets/Icons/Shop.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new SupermarketFactory(),
                "Супермаркет",
                "Assets/Icons/Supermarket.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new CafeFactory(),
                "Кафе",
                "Assets/Icons/Cafe.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new RestaurantFactory(),
                "Ресторан",
                "Assets/Icons/Restaurant.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new GasStationFactory(),
                "Заправка",
                "Assets/Icons/GasStation.png"
            ));

            #region Industrial Buildings ViewModels
            // Промышленные
            var industrial = new BuildingCategory { Name = "Промышленные" };

            industrial.Objects.Add(new ObjectVM(
                new FactoryBuildingFactory(),
                "Завод",
                "Assets/Icons/Factory.png"
            ));

            industrial.Objects.Add(new ObjectVM(
                new WarehouseFactory(),
                "Склад",
                "Assets/Icons/Warehouse.png"
            ));
            #endregion

            // Инфраструктура
            var infrastructure = new BuildingCategory { Name = "Инфраструктура" };

            infrastructure.Objects.Add(new ObjectVM(
                new UrbanParkFactory(),
                "Городской парк",
                "Assets/Icons/UrbanPark.png"
            ));

            infrastructure.Objects.Add(new ObjectVM(
                new SquareParkFactory(),
                    "Сквер",
                    "Assets/Icons/Square.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new BotanicalGardenParkFactory(),
                    "Ботанический сад",
                    "Assets/Icons/BotanicalGarden.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new PlaygroundParkFactory(),
                    "Детская площадка",
                    "Assets/Icons/Playground.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new RecreationAreaParkFactory(),
                    "Зона отдыха",
                    "Assets/Icons/RecreationArea.png"
                )
            );

            infrastructure.Objects.Add(new ObjectVM(
                new RoadFactory(),
                "Дорога",
                "Assets/Icons/Road.png"
            ));

            infrastructure.Objects.Add(new ObjectVM(
            new PedestrianPathFactory(),
            "Пешеходная дорожка",
            "Assets/Icons/PedestrianPath.png" 
            ));

            infrastructure.Objects.Add(new ObjectVM(
                new BicyclePathFactory(),
                "Велосипедная дорожка",
                "Assets/Icons/BicyclePath.png" 
            ));

            // Добавление категорий в реестр
            Categories.Add(infrastructure);
            Categories.Add(residential);
            Categories.Add(commercial);
            Categories.Add(industrial);
        }
    }
}
