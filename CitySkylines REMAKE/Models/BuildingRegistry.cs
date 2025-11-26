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

            residential.Buildings.Add(
                new BuildingVM(
                    new ResidentialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(2, 2)
                    ),
                    "Маленький дом",
                    "Типа путь до иконки"
                )
            );

            residential.Buildings.Add(
                new BuildingVM(
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

            commercial.Buildings.Add(
                new BuildingVM(
                    new CommercialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(3, 2)
                    ),
                    "Магазин",
                    "Типа путь до иконки"
                )
            );

            commercial.Buildings.Add(
                new BuildingVM(
                    new CommercialBuilding(
                        floors: 1, 
                        maxOccupancy: 1,
                        area: new Area(3, 3)
                    ),
                    "Офис",
                    "Типа путь до иконки"
                )
            );

            var industrial = new BuildingCategory { Name = "Промышленные" };

            industrial.Buildings.Add(
                new BuildingVM(
                    new IndustrialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(5, 5)
                    ),
                    "Завод",
                    "Assets/Icons/Factory.png"
                )
            );

            industrial.Buildings.Add(
                new BuildingVM(
                    new IndustrialBuilding(
                        floors: 1,
                        maxOccupancy: 1,
                        area: new Area(4, 6)
                    ),
                    "Склад",
                    "Assets/Icons/Warehouse.png"
                )
            );
            
            var infrastructure = new BuildingCategory { Name = "Парки" };

            infrastructure.Buildings.Add(
                new BuildingVM(
                    new Park(
                        area: new Area(3, 3),
                        type: ParkType.UrbanPark
                    ),
                    "Городской парк",
                    "Assets/Icons/UrbanPark.png"
                )
            );

            infrastructure.Buildings.Add(
                new BuildingVM(
                    new Park(
                        area: new Area(2, 2),
                        type: ParkType.Square
                    ),
                    "Сквер",
                    "Assets/Icons/Square.png"
                )
            );

            infrastructure.Buildings.Add(
                new BuildingVM(
                    new Park(
                        area: new Area(4, 4),
                        type: ParkType.BotanicalGarden
                    ),
                    "Ботанический сад",
                    "Assets/Icons/BotanicalGarden.png"
                )
            );

            infrastructure.Buildings.Add(
                new BuildingVM(
                    new Park(
                        area: new Area(1, 1),
                        type: ParkType.Playground
                    ),
                    "Детская площадка",
                    "Assets/Icons/Playground.png"
                )
            );

            infrastructure.Buildings.Add(
                new BuildingVM(
                    new Park(
                        area: new Area(2, 3),
                        type: ParkType.RecreationArea
                    ),
                    "Зона отдыха",
                    "Assets/Icons/RecreationArea.png"
                )
            );
            
            infrastructure.Buildings.Add(new BuildingVM(
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
