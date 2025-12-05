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
                "/Icons/SmallResidentialBuilding.png"
            ));

            residential.Objects.Add(new ObjectVM(
                new ApartmentFactory(),
                "Многоквартирный дом",
                "/Icons/HighResidentialBuilding.png"
            ));

            var commercial = new BuildingCategory { Name = "Коммерческие" };
            // Коммерческие
            commercial.Objects.Add(new ObjectVM(
                new PharmacyFactory(),
                "Аптека",
                ""
            ));

            commercial.Objects.Add(new ObjectVM(
                new ShopFactory(),
                "Магазин",
                "/Icons/Shop.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new SupermarketFactory(),
                "Супермаркет",
                ""
            ));

            commercial.Objects.Add(new ObjectVM(
                new CafeFactory(),
                "Кафе",
                ""
            ));

            commercial.Objects.Add(new ObjectVM(
                new RestaurantFactory(),
                "Ресторан",
                ""
            ));

            commercial.Objects.Add(new ObjectVM(
                new GasStationFactory(),
                "Заправка",
                ""
            ));

            // Промышленные
            var industrial = new BuildingCategory { Name = "Промышленные" };

            industrial.Objects.Add(new ObjectVM(
                new FactoryBuildingFactory(),
                "Завод",
                "/Icons/Factory.png"
            ));

            industrial.Objects.Add(new ObjectVM(
                new WarehouseFactory(),
                "Склад",
                "/Icons/Warehouse.png"
            ));

            // Инфраструктура
            var infrastructure = new BuildingCategory { Name = "Инфраструктура" };

            infrastructure.Objects.Add(new ObjectVM(
                new UrbanParkFactory(),
                "Городской парк",
                "/Icons/UrbanPark.png"
            ));

            infrastructure.Objects.Add(new ObjectVM(
                new SquareParkFactory(),
                    "Сквер",
                    "/Icons/Square.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new BotanicalGardenParkFactory(),
                    "Ботанический сад",
                    "/Icons/BotanGarden.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new PlaygroundParkFactory(),
                    "Детская площадка",
                    "/Icons/ChildPlayground.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new RecreationAreaParkFactory(),
                    "Зона отдыха",
                    "/Icons/RestArea.png"
                )
            );

            infrastructure.Objects.Add(new ObjectVM(
                new RoadFactory(),
                "Дорога",
                "/Icons/Road.png"
            ));

            infrastructure.Objects.Add(new ObjectVM(
                new PedestrianPathFactory(),
                "Пешеходная дорожка",
                ""
            ));

            infrastructure.Objects.Add(new ObjectVM(
                new BicyclePathFactory(),
                "Велосипедная дорожка",
                ""
            ));

            // Добавление категорий в реестр
            Categories.Add(infrastructure);
            Categories.Add(residential);
            Categories.Add(commercial);
            Categories.Add(industrial);
        }
    }
}
