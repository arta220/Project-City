using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Buildings.Utility;
using Domain.Base;
using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Factories;
using Domain.Infrastructure;
using Domain.Map;
using Services.PlaceBuilding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Services.SaveLoad
{
    /// <summary>
    /// Реализация сервиса сохранения и загрузки игры.
    /// 
    /// КОНЦЕПЦИЯ:
    /// Сервис отвечает за сохранение текущего состояния игры в файл и загрузку состояния из файла.
    /// 
    /// ПРОЦЕСС СОХРАНЕНИЯ:
    /// 1. Получает все здания из Simulation через GetAllBuildings()
    /// 2. Для каждого здания определяет его тип через GetBuildingType()
    /// 3. Сохраняет позицию и размер каждого здания в SaveData
    /// 4. Сериализует SaveData в JSON и записывает в файл
    /// 
    /// ПРОЦЕСС ЗАГРУЗКИ:
    /// 1. Читает JSON файл и десериализует в SaveData
    /// 2. Очищает карту через Simulation.ClearMap()
    /// 3. Для каждого сохраненного здания:
    ///    - Находит соответствующую фабрику в _factoryMap
    ///    - Создает здание через factory.Create()
    ///    - Размещает здание на карте через Simulation.TryPlace()
    /// 
    /// ОСОБЕННОСТИ:
    /// - Сохраняются только здания и их позиции (не сохраняются материалы, продукты, рабочие)
    /// - При загрузке здания создаются заново через фабрики (с начальными значениями)
    /// - Используется карта фабрик (_factoryMap) для восстановления типов зданий
    /// </summary>
    public class SaveLoadService : ISaveLoadService
    {
        /// <summary>
        /// Карта фабрик для загрузки зданий.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Словарь, где ключ - имя фабрики (например, "ResourceExtractionFactory"),
        /// значение - экземпляр фабрики для создания здания.
        /// 
        /// ИСПОЛЬЗОВАНИЕ:
        /// При загрузке игры по имени фабрики находится соответствующий экземпляр,
        /// который создает здание через Create().
        /// 
        /// ПРИМЕР:
        /// SaveData содержит BuildingType = "ResourceExtractionFactory"
        /// → _factoryMap["ResourceExtractionFactory"] → factory.Create() → новое здание
        /// </summary>
        private readonly Dictionary<string, IMapObjectFactory> _factoryMap;

        /// <summary>
        /// Инициализирует сервис сохранения и загрузки.
        /// 
        /// Создает карту всех доступных фабрик для восстановления зданий при загрузке.
        /// Каждая фабрика регистрируется под своим именем (nameof(FactoryName)).
        /// </summary>
        public SaveLoadService()
        {
            // Создаем карту фабрик для загрузки
            // Ключ - имя фабрики, значение - экземпляр фабрики
            _factoryMap = new Dictionary<string, IMapObjectFactory>
            {
                { nameof(SmallHouseFactory), new SmallHouseFactory() },
                { nameof(ApartmentFactory), new ApartmentFactory() },
                { nameof(FactoryBuildingFactory), new FactoryBuildingFactory() },
                { nameof(WarehouseFactory), new WarehouseFactory() },
                { nameof(CardboardFactory), new CardboardFactory() },
                { nameof(PackagingFactory), new PackagingFactory() },
                { nameof(PharmaceuticalFactoryFactory), new PharmaceuticalFactoryFactory() },
                { nameof(RecyclingPlantFactoryFactory), new RecyclingPlantFactoryFactory() },
                { nameof(MiningFactoryFactory), new MiningFactoryFactory() },
                { nameof(WoodProcessingFactoryFactory), new WoodProcessingFactoryFactory() },
                { nameof(ResourceExtractionFactory), new ResourceExtractionFactory() },
                { nameof(WoodProcessingFactory), new WoodProcessingFactory() },
                { nameof(RecyclingFactory), new RecyclingFactory() },
                { nameof(UrbanParkFactory), new UrbanParkFactory() },
                { nameof(SquareParkFactory), new SquareParkFactory() },
                { nameof(BotanicalGardenParkFactory), new BotanicalGardenParkFactory() },
                { nameof(PlaygroundParkFactory), new PlaygroundParkFactory() },
                { nameof(RecreationAreaParkFactory), new RecreationAreaParkFactory() },
                { nameof(UtilityOfficeFactory), new UtilityOfficeFactory() },
                { nameof(AirPortFactory), new AirPortFactory() },
            };
        }

        /// <summary>
        /// Сохраняет текущее состояние игры в файл.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Собирает информацию о всех зданиях на карте и сохраняет их в JSON файл.
        /// 
        /// ПРОЦЕСС:
        /// 1. Получает все здания через simulation.GetAllBuildings()
        /// 2. Для каждого здания получает его позицию через simulation.GetMapObjectPlacement()
        /// 3. Определяет тип здания через GetBuildingType() (имя фабрики)
        /// 4. Сохраняет данные о здании (тип, позиция X/Y, размер Width/Height) в SaveData
        /// 5. Сериализует SaveData в JSON и записывает в файл
        /// 
        /// ЧТО СОХРАНЯЕТСЯ:
        /// - Тип здания (имя фабрики, например "ResourceExtractionFactory")
        /// - Позиция на карте (X, Y)
        /// - Размер здания (Width, Height)
        /// 
        /// ЧТО НЕ СОХРАНЯЕТСЯ:
        /// - Материалы и продукты в MaterialsBank/ProductsBank
        /// - Рабочие и их состояние
        /// - Внутреннее состояние зданий
        /// 
        /// ПРИ ЗАГРУЗКЕ:
        /// Здания создаются заново через фабрики с начальными значениями.
        /// </summary>
        /// <param name="filePath">Путь к файлу для сохранения (обычно .json)</param>
        /// <param name="simulation">Объект симуляции, содержащий все здания</param>
        /// <returns>True, если сохранение успешно, иначе false</returns>
        public bool SaveGame(string filePath, Simulation simulation)
        {
            try
            {
                // Создаем объект для хранения данных сохранения
                var saveData = new SaveData();

                // Шаг 1: Получаем все здания из репозитория размещения
                // GetAllBuildings() возвращает все здания, размещенные на карте
                var allBuildings = simulation.GetAllBuildings().ToList();
                System.Diagnostics.Debug.WriteLine($"Сохранение: найдено {allBuildings.Count} зданий");

                // Шаг 2: Обрабатываем каждое здание
                foreach (var building in allBuildings)
                {
                    // Получаем позицию размещения здания на карте
                    var (placement, found) = simulation.GetMapObjectPlacement(building);
                    if (!found || placement == null)
                    {
                        // Если размещение не найдено, пропускаем это здание
                        System.Diagnostics.Debug.WriteLine($"Сохранение: не найдено размещение для здания {building.GetType().Name}");
                        continue;
                    }

                    // Шаг 3: Определяем тип здания (имя фабрики для восстановления)
                    // GetBuildingType() анализирует свойства здания и возвращает имя фабрики
                    string buildingType = GetBuildingType(building);
                    if (string.IsNullOrEmpty(buildingType))
                    {
                        // Если тип не определен, пропускаем это здание
                        System.Diagnostics.Debug.WriteLine($"Сохранение: не удалось определить тип здания {building.GetType().Name}");
                        continue;
                    }

                    // Шаг 4: Сохраняем данные о здании
                    // Сохраняем тип, позицию и размер для последующего восстановления
                    saveData.Buildings.Add(new SaveData.BuildingSaveData
                    {
                        BuildingType = buildingType, // Имя фабрики (например, "ResourceExtractionFactory")
                        X = placement.Value.Position.X, // Координата X на карте
                        Y = placement.Value.Position.Y, // Координата Y на карте
                        Width = placement.Value.Area.Width, // Ширина здания
                        Height = placement.Value.Area.Height // Высота здания
                    });
                    System.Diagnostics.Debug.WriteLine($"Сохранение: добавлено здание {buildingType} на позиции ({placement.Value.Position.X}, {placement.Value.Position.Y})");
                }

                // Шаг 5: Сериализуем данные в JSON и сохраняем в файл
                // WriteIndented = true делает JSON читаемым (с отступами)
                var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                System.Diagnostics.Debug.WriteLine($"Сохранение: сохранено {saveData.Buildings.Count} зданий в файл {filePath}");

                return true;
            }
            catch (Exception ex)
            {
                // Обработка ошибок: логируем и возвращаем false
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Загружает состояние игры из файла.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Читает сохраненные данные из JSON файла и восстанавливает здания на карте.
        /// 
        /// ПРОЦЕСС:
        /// 1. Проверяет существование файла
        /// 2. Читает и десериализует JSON в SaveData
        /// 3. Очищает карту через simulation.ClearMap() (удаляет все текущие здания)
        /// 4. Для каждого сохраненного здания:
        ///    - Находит соответствующую фабрику в _factoryMap по имени
        ///    - Создает здание через factory.Create() (с начальными значениями)
        ///    - Размещает здание на карте через simulation.TryPlace()
        /// 
        /// ВОССТАНОВЛЕНИЕ ЗДАНИЙ:
        /// - Здания создаются заново через фабрики (с начальными MaterialsBank, ProductsBank)
        /// - Позиция и размер восстанавливаются из сохраненных данных
        /// - Внутреннее состояние (материалы, продукты, рабочие) не восстанавливается
        /// 
        /// ОСОБЕННОСТИ:
        /// - Если фабрика не найдена в _factoryMap, здание пропускается
        /// - Если размещение не удалось (TryPlace вернул false), здание пропускается
        /// - Возвращает true даже если не все здания загружены (частичная загрузка)
        /// </summary>
        /// <param name="filePath">Путь к файлу сохранения (.json)</param>
        /// <param name="simulation">Объект симуляции, в который загружаются здания</param>
        /// <returns>True, если загрузка началась успешно, иначе false</returns>
        public bool LoadGame(string filePath, Simulation simulation)
        {
            try
            {
                // Шаг 1: Проверяем существование файла
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"Загрузка: файл {filePath} не существует");
                    return false;
                }

                // Шаг 2: Читаем файл и десериализуем JSON в SaveData
                var json = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<SaveData>(json);

                if (saveData == null)
                {
                    // Если десериализация не удалась, файл поврежден или пуст
                    System.Diagnostics.Debug.WriteLine("Загрузка: не удалось десериализовать данные");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Загрузка: найдено {saveData.Buildings.Count} зданий для загрузки");

                // Шаг 3: Очищаем карту перед загрузкой
                // Это удаляет все текущие здания и обновляет UI через события
                simulation.ClearMap();

                int loadedCount = 0;
                // Шаг 4: Загружаем каждое здание
                foreach (var buildingData in saveData.Buildings)
                {
                    // Находим фабрику по имени типа здания
                    // buildingData.BuildingType содержит имя фабрики (например, "ResourceExtractionFactory")
                    if (!_factoryMap.TryGetValue(buildingData.BuildingType, out var factory))
                    {
                        // Если фабрика не найдена, пропускаем это здание
                        System.Diagnostics.Debug.WriteLine($"Загрузка: фабрика {buildingData.BuildingType} не найдена");
                        continue;
                    }

                    // Создаем здание через фабрику (с начальными значениями)
                    // factory.Create() создает новое здание с начальными MaterialsBank, ProductsBank
                    var building = factory.Create();
                    
                    // Восстанавливаем размер и позицию из сохраненных данных
                    var area = new Area(buildingData.Width, buildingData.Height);
                    var placement = new Placement(new Position(buildingData.X, buildingData.Y), area);

                    // Пытаемся разместить здание на карте
                    if (simulation.TryPlace(building, placement))
                    {
                        loadedCount++;
                        System.Diagnostics.Debug.WriteLine($"Загрузка: размещено здание {buildingData.BuildingType} на позиции ({buildingData.X}, {buildingData.Y})");
                    }
                    else
                    {
                        // Если размещение не удалось (например, позиция занята), пропускаем
                        System.Diagnostics.Debug.WriteLine($"Загрузка: не удалось разместить здание {buildingData.BuildingType} на позиции ({buildingData.X}, {buildingData.Y})");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Загрузка: успешно загружено {loadedCount} из {saveData.Buildings.Count} зданий");
                return true;
            }
            catch (Exception ex)
            {
                // Обработка ошибок: логируем и возвращаем false
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Определяет тип здания (имя фабрики) для сохранения.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Анализирует свойства здания и возвращает имя фабрики, которая может создать такое же здание.
        /// Используется при сохранении для идентификации типа здания.
        /// 
        /// АЛГОРИТМ:
        /// Использует pattern matching для определения типа:
        /// - Для ResidentialBuilding: проверяет MaxOccupancy
        /// - Для IndustrialBuilding: анализирует Workshops (входные/выходные материалы)
        /// - Для Park: проверяет ParkType
        /// - Для других типов: проверяет конкретный тип
        /// 
        /// ОПРЕДЕЛЕНИЕ ПРОМЫШЛЕННЫХ ЗАВОДОВ:
        /// Анализирует цехи (Workshops) для определения типа:
        /// - ResourceExtractionFactory: цех с InputMaterial="None" и OutputProduct="Iron/Wood/Coal"
        /// - WoodProcessingFactory: цех с OutputProduct="Lumber" или InputMaterial="Wood" и OutputProduct="Furniture"
        /// - RecyclingFactory: цех с OutputProduct="Steel/Plastic/Fuel"
        /// 
        /// ВОЗВРАЩАЕМОЕ ЗНАЧЕНИЕ:
        /// - Имя фабрики (например, "ResourceExtractionFactory") - если тип определен
        /// - Пустая строка - если тип не определен (здание не будет сохранено)
        /// </summary>
        /// <param name="building">Здание для анализа</param>
        /// <returns>Имя фабрики для создания такого же здания, или пустая строка</returns>
        private string GetBuildingType(MapObject building)
        {
            return building switch
            {
                // Жилые здания определяются по максимальной вместимости
                ResidentialBuilding rb when rb.MaxOccupancy == 50 => nameof(SmallHouseFactory),
                ResidentialBuilding rb when rb.MaxOccupancy == 250 => nameof(ApartmentFactory),
                
                // Промышленные здания определяются по типу и цехам
                IndustrialBuilding ib when ib.Type == IndustrialBuildingType.Factory && ib.Workshops.Count == 0 => nameof(FactoryBuildingFactory),
                IndustrialBuilding ib when ib.Type == IndustrialBuildingType.Warehouse && ib.Workshops.Any(w => w.InputMaterial.ToString() == "Wood") => nameof(WarehouseFactory),
                
                // Заводы определяются по производимым продуктам
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "CardboardSheets") => nameof(CardboardFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "CardboardBox") => nameof(PackagingFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Medicine") => nameof(PharmaceuticalFactoryFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.InputMaterial.ToString() == "PlasticWaste") => nameof(RecyclingPlantFactoryFactory),
                
                // ДОБЫВАЮЩАЯ ПРОМЫШЛЕННОСТЬ: цех с InputMaterial="None" и OutputProduct="Iron/Wood/Coal"
                IndustrialBuilding ib when ib.Workshops.Any(w => w.InputMaterial.ToString() == "None" && 
                    (w.OutputProduct.ToString() == "Iron" || w.OutputProduct.ToString() == "Wood" || w.OutputProduct.ToString() == "Coal")) 
                    => nameof(ResourceExtractionFactory),
                
                // ДЕРЕВООБРАБАТЫВАЮЩАЯ ПРОМЫШЛЕННОСТЬ: цех производит Lumber или использует Wood для Furniture
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Lumber" || 
                    (w.InputMaterial.ToString() == "Wood" && w.OutputProduct.ToString() == "Furniture")) 
                    => nameof(WoodProcessingFactory),
                
                // ПЕРЕРАБАТЫВАЮЩАЯ ПРОМЫШЛЕННОСТЬ: цех производит Steel, Plastic или Fuel
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Steel" || 
                    w.OutputProduct.ToString() == "Plastic" || w.OutputProduct.ToString() == "Fuel") 
                    => nameof(RecyclingFactory),
                
                // Другие промышленные заводы
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Iron" || 
                    w.OutputProduct.ToString() == "Copper" || w.OutputProduct.ToString() == "Oil" || w.OutputProduct.ToString() == "Gas") 
                    => nameof(MiningFactoryFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.InputMaterial.ToString() == "Wood" && 
                    (w.OutputProduct.ToString() == "Paper" || w.OutputProduct.ToString() == "Furniture" || w.OutputProduct.ToString() == "WoodChips")) 
                    => nameof(WoodProcessingFactoryFactory),
                
                // Парки определяются по типу
                Park park when park.Type == ParkType.UrbanPark => nameof(UrbanParkFactory),
                Park park when park.Type == ParkType.Square => nameof(SquareParkFactory),
                Park park when park.Type == ParkType.BotanicalGarden => nameof(BotanicalGardenParkFactory),
                Park park when park.Type == ParkType.Playground => nameof(PlaygroundParkFactory),
                Park park when park.Type == ParkType.RecreationArea => nameof(RecreationAreaParkFactory),
                
                // Другие типы зданий
                UtilityOffice => nameof(UtilityOfficeFactory),
                Port port when port.Type == PortType.AirPort => nameof(AirPortFactory),
                
                // Тип не определен - здание не будет сохранено
                _ => string.Empty
            };
        }
    }
}

