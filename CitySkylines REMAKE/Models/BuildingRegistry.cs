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
                "/Icons/Pharmacy.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new ShopFactory(),
                "Магазин",
                "/Icons/Shop.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new SupermarketFactory(),
                "Супермаркет",
                "/Icons/Supermarket.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new CafeFactory(),
                "Кафе",
                "/Icons/Cafe.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new RestaurantFactory(),
                "Ресторан",
                "/Icons/Restaurant.png"
            ));

            commercial.Objects.Add(new ObjectVM(
                new GasStationFactory(),
                "Заправка",
                "/Icons/GasStation.png"
            ));

            #region Industrial Buildings ViewModels
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

            //Фармацевтический завод
            industrial.Objects.Add(new ObjectVM(
                new PharmaceuticalFactoryFactory(),
                "Фармацевтический завод",
                "Assets/Icons/PharmaceuticalFactory.png"
            ));

            //Завод по переработке отходов и вторичной переработке
            industrial.Objects.Add(new ObjectVM(
                new RecyclingPlantFactoryFactory(),
                "Завод по переработке отходов и вторичной переработке",
                "Assets/Icons/RecyclingPlantFactory.png"
                            ));

            industrial.Objects.Add(new ObjectVM(
                new CardboardFactory(),
                "Завод картона",
                "Assets/Icons/FactoryCardBoard.png"
            ));

            industrial.Objects.Add(new ObjectVM(
                new PackagingFactory(),
                "Завод упаковки",
                "Assets/Icons/FactoryPacking.png"
            ));
            #endregion

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

            // Офис ЖКХ
            infrastructure.Objects.Add(new ObjectVM(
                new UtilityOfficeFactory(),
                "Офис ЖКХ",
                "Assets/Icons/UtilityOffice.png" // Нужно будет добавить иконку
            ));

            infrastructure.Objects.Add(new ObjectVM(
                new AirPortFactory(),
                    "Аэрпорт",
                    "Assets/Icons/AirPort.png"
                )
            );

            // Добавление категорий в реестр
            Categories.Add(infrastructure);
            Categories.Add(residential);
            Categories.Add(commercial);
            Categories.Add(industrial);
        }
    }
}
