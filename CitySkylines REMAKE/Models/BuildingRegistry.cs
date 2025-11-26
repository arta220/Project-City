using System.Collections.ObjectModel;
using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Domain.Map;

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
            var residential = new BuildingCategory { Name = "Жилые" };

            residential.Objects.Add(
                new ObjectVM(
                    new ResidentialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(2, 2)
                    ),
                    "Маленький дом",
                    "Типа путь до иконки"
                )
            );

            residential.Objects.Add(
                new ObjectVM(
                    new ResidentialBuilding(
                        floors: 5,
                        maxOccupancy: 1,
                        area: new Area(4, 4)
                    ),
                    "Многоквартирный дом",
                    "Типа путь до иконки"
                )
            );

            var commercial = new BuildingCategory { Name = "Коммерческие" };

            // Магазин
            commercial.Objects.Add(
                new ObjectVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(2, 2),
                        type: CommercialType.Shop,
                        requiredWorkers: 2,
                        maxVisitors: 8
                    ),
                    "Магазин",
                    "Путь к Shop.png"
                )
            );

            // Супермаркет
            commercial.Objects.Add(
                new ObjectVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(3, 3),
                        type: CommercialType.Supermarket,
                        requiredWorkers: 6,
                        maxVisitors: 20
                    ),
                    "Супермаркет",
                    "Путь к Supermarket.png"
                )
            );

            // Кафе
            commercial.Objects.Add(
                new ObjectVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(2, 2),
                        type: CommercialType.Cafe,
                        requiredWorkers: 4,
                        maxVisitors: 10
                    ),
                    "Кафе",
                    "Путь к Cafe.png"
                )
            );

            // Ресторан
            commercial.Objects.Add(
                new ObjectVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(3, 3),
                        type: CommercialType.Restaurant,
                        requiredWorkers: 6,
                        maxVisitors: 20
                    ),
                    "Ресторан",
                    "Путь к Restaurant.png"
                )
            );

            // Заправка
            commercial.Objects.Add(
                new ObjectVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(2, 2),
                        type: CommercialType.GasStation,
                        requiredWorkers: 3,
                        maxVisitors: 4
                    ),
                    "Заправка",
                    "Путь к GasStation.png"
                )
            );

            // Аптека
            commercial.Objects.Add(
                new ObjectVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(2, 2),
                        type: CommercialType.Pharmacy,
                        requiredWorkers: 2,
                        maxVisitors: 6
                    ),
                    "Аптека",
                    "Путь к Pharmacy.png"
                )
            );

            var industrial = new BuildingCategory { Name = "Промышленные" };

            industrial.Objects.Add(
                new ObjectVM(
                    new IndustrialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(5, 5)
                    ),
                    "Завод",
                    "Assets/Icons/Factory.png"
                )
            );

            industrial.Objects.Add(
                new ObjectVM(
                    new IndustrialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(4, 6)
                    ),
                    "Склад",
                    "Assets/Icons/Warehouse.png"
                )
            );

            var infrastructure = new BuildingCategory { Name = "Инфраструктура" };

            infrastructure.Objects.Add(
                new ObjectVM(
                    new Park(
                        area: new Area(3, 3),
                        type: ParkType.UrbanPark
                    ),
                    "Городской парк",
                    "Assets/Icons/UrbanPark.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new Park(
                        area: new Area(2, 2),
                        type: ParkType.Square
                    ),
                    "Сквер",
                    "Assets/Icons/Square.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new Park(
                        area: new Area(4, 4),
                        type: ParkType.BotanicalGarden
                    ),
                    "Ботанический сад",
                    "Assets/Icons/BotanicalGarden.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new Park(
                        area: new Area(1, 1),
                        type: ParkType.Playground
                    ),
                    "Детская площадка",
                    "Assets/Icons/Playground.png"
                )
            );

            infrastructure.Objects.Add(
                new ObjectVM(
                    new Park(
                        area: new Area(2, 3),
                        type: ParkType.RecreationArea
                    ),
                    "Зона отдыха",
                    "Assets/Icons/RecreationArea.png"
                )
            );

            infrastructure.Objects.Add(new ObjectVM(
                new Road(
                area: new Area(1, 1)),
                "Дорога",
                "Assets/Icons/RecreationArea.png"
                ));



            Categories.Add(infrastructure);
            Categories.Add(residential);
            Categories.Add(commercial);
            Categories.Add(industrial);
        }
    }
}
