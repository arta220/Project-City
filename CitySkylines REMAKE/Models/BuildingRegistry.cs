using System.Collections.ObjectModel;
using CitySimulatorWPF.ViewModels;
using Domain.Base;
using Domain.Buildings;
using Domain.Enums;
using Domain.Map;

namespace CitySimulatorWPF.Models
{
    /// <summary>
    /// Центральный реестр всех типов зданий и инфраструктурных объектов.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Хранит и группирует доступные объекты для строительства по категориям.
    /// - Обеспечивает единый источник данных для UI (панель строительства, MapVM и т.д.).
    ///
    /// Контекст использования:
    /// - Панель выбора зданий (BuildingPanelViewModel) использует Categories для отображения доступных зданий.
    /// - MapVM использует выбранный объект для постановки на карту.
    ///
    /// Расширяемость:
    /// - Можно добавлять новые категории и объекты без изменения остального кода.
    /// - Можно подключить конфигурацию из файла (JSON, XML) вместо хардкода.
    /// </remarks>
    public static class BuildingRegistry
    {
        /// <summary>
        /// Список категорий зданий и инфраструктуры.
        /// </summary>
        public static ObservableCollection<BuildingCategory> Categories { get; private set; }

        static BuildingRegistry()
        {
            Categories = new ObservableCollection<BuildingCategory>();
            Initialize();
        }

        private static void Initialize()
        {
            // Жилые здания
            var residential = new BuildingCategory { Name = "Жилые" };
            residential.Objects.Add(new ObjectVM(
                new ResidentialBuilding(1, 1, new Area(2, 2)),
                "Маленький дом",
                "Assets/Icons/SmallHouse.png"));
            residential.Objects.Add(new ObjectVM(
                new ResidentialBuilding(5, 1, new Area(4, 4)),
                "Многоквартирный дом",
                "Assets/Icons/Apartment.png"));

            // Коммерческие здания
            var commercial = new BuildingCategory { Name = "Коммерческие" };
            commercial.Objects.Add(new ObjectVM(
                new CommercialBuilding(1, 1, new Area(3, 2)),
                "Магазин",
                "Assets/Icons/Shop.png"));
            commercial.Objects.Add(new ObjectVM(
                new CommercialBuilding(1, 1, new Area(3, 3)),
                "Офис",
                "Assets/Icons/Office.png"));

            // Промышленные здания
            var industrial = new BuildingCategory { Name = "Промышленные" };
            industrial.Objects.Add(new ObjectVM(
                new IndustrialBuilding(1, 1, new Area(5, 5)),
                "Завод",
                "Assets/Icons/Factory.png"));
            industrial.Objects.Add(new ObjectVM(
                new IndustrialBuilding(1, 1, new Area(4, 6)),
                "Склад",
                "Assets/Icons/Warehouse.png"));

            // Инфраструктура
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
                new Road(new Area(1, 1)),
                "Дорога",
                "Assets/Icons/Road.png"));

            // Добавление категорий в реестр
            Categories.Add(infrastructure);
            Categories.Add(residential);
            Categories.Add(commercial);
            Categories.Add(industrial);
        }
    }
}
