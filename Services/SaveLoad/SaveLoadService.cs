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
    /// </summary>
    public class SaveLoadService : ISaveLoadService
    {
        private readonly Dictionary<string, IMapObjectFactory> _factoryMap;

        public SaveLoadService()
        {
            // Создаем карту фабрик для загрузки
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

        public bool SaveGame(string filePath, Simulation simulation)
        {
            try
            {
                var saveData = new SaveData();

                // Получаем все здания из репозитория
                var allBuildings = simulation.GetAllBuildings().ToList();
                System.Diagnostics.Debug.WriteLine($"Сохранение: найдено {allBuildings.Count} зданий");

                foreach (var building in allBuildings)
                {
                    var (placement, found) = simulation.GetMapObjectPlacement(building);
                    if (!found || placement == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Сохранение: не найдено размещение для здания {building.GetType().Name}");
                        continue;
                    }

                    // Определяем тип здания
                    string buildingType = GetBuildingType(building);
                    if (string.IsNullOrEmpty(buildingType))
                    {
                        System.Diagnostics.Debug.WriteLine($"Сохранение: не удалось определить тип здания {building.GetType().Name}");
                        continue;
                    }

                    saveData.Buildings.Add(new SaveData.BuildingSaveData
                    {
                        BuildingType = buildingType,
                        X = placement.Value.Position.X,
                        Y = placement.Value.Position.Y,
                        Width = placement.Value.Area.Width,
                        Height = placement.Value.Area.Height
                    });
                    System.Diagnostics.Debug.WriteLine($"Сохранение: добавлено здание {buildingType} на позиции ({placement.Value.Position.X}, {placement.Value.Position.Y})");
                }

                // Сериализуем и сохраняем
                var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                System.Diagnostics.Debug.WriteLine($"Сохранение: сохранено {saveData.Buildings.Count} зданий в файл {filePath}");

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        public bool LoadGame(string filePath, Simulation simulation)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"Загрузка: файл {filePath} не существует");
                    return false;
                }

                // Читаем файл
                var json = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<SaveData>(json);

                if (saveData == null)
                {
                    System.Diagnostics.Debug.WriteLine("Загрузка: не удалось десериализовать данные");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Загрузка: найдено {saveData.Buildings.Count} зданий для загрузки");

                // Очищаем карту перед загрузкой (это также обновит UI через события)
                simulation.ClearMap();

                int loadedCount = 0;
                // Загружаем здания
                foreach (var buildingData in saveData.Buildings)
                {
                    if (!_factoryMap.TryGetValue(buildingData.BuildingType, out var factory))
                    {
                        System.Diagnostics.Debug.WriteLine($"Загрузка: фабрика {buildingData.BuildingType} не найдена");
                        continue;
                    }

                    var building = factory.Create();
                    var area = new Area(buildingData.Width, buildingData.Height);
                    var placement = new Placement(new Position(buildingData.X, buildingData.Y), area);

                    if (simulation.TryPlace(building, placement))
                    {
                        loadedCount++;
                        System.Diagnostics.Debug.WriteLine($"Загрузка: размещено здание {buildingData.BuildingType} на позиции ({buildingData.X}, {buildingData.Y})");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Загрузка: не удалось разместить здание {buildingData.BuildingType} на позиции ({buildingData.X}, {buildingData.Y})");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Загрузка: успешно загружено {loadedCount} из {saveData.Buildings.Count} зданий");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private string GetBuildingType(MapObject building)
        {
            return building switch
            {
                ResidentialBuilding rb when rb.MaxOccupancy == 50 => nameof(SmallHouseFactory),
                ResidentialBuilding rb when rb.MaxOccupancy == 250 => nameof(ApartmentFactory),
                IndustrialBuilding ib when ib.Type == IndustrialBuildingType.Factory && ib.Workshops.Count == 0 => nameof(FactoryBuildingFactory),
                IndustrialBuilding ib when ib.Type == IndustrialBuildingType.Warehouse && ib.Workshops.Any(w => w.InputMaterial.ToString() == "Wood") => nameof(WarehouseFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "CardboardSheets") => nameof(CardboardFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "CardboardBox") => nameof(PackagingFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Medicine") => nameof(PharmaceuticalFactoryFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.InputMaterial.ToString() == "PlasticWaste") => nameof(RecyclingPlantFactoryFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.InputMaterial.ToString() == "None" && (w.OutputProduct.ToString() == "Iron" || w.OutputProduct.ToString() == "Wood" || w.OutputProduct.ToString() == "Coal")) => nameof(ResourceExtractionFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Lumber" || (w.InputMaterial.ToString() == "Wood" && w.OutputProduct.ToString() == "Furniture")) => nameof(WoodProcessingFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Steel" || w.OutputProduct.ToString() == "Plastic" || w.OutputProduct.ToString() == "Fuel") => nameof(RecyclingFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.OutputProduct.ToString() == "Iron" || w.OutputProduct.ToString() == "Copper" || w.OutputProduct.ToString() == "Oil" || w.OutputProduct.ToString() == "Gas") => nameof(MiningFactoryFactory),
                IndustrialBuilding ib when ib.Workshops.Any(w => w.InputMaterial.ToString() == "Wood" && (w.OutputProduct.ToString() == "Paper" || w.OutputProduct.ToString() == "Furniture" || w.OutputProduct.ToString() == "WoodChips")) => nameof(WoodProcessingFactoryFactory),
                Park park when park.Type == ParkType.UrbanPark => nameof(UrbanParkFactory),
                Park park when park.Type == ParkType.Square => nameof(SquareParkFactory),
                Park park when park.Type == ParkType.BotanicalGarden => nameof(BotanicalGardenParkFactory),
                Park park when park.Type == ParkType.Playground => nameof(PlaygroundParkFactory),
                Park park when park.Type == ParkType.RecreationArea => nameof(RecreationAreaParkFactory),
                UtilityOffice => nameof(UtilityOfficeFactory),
                Port port when port.Type == PortType.AirPort => nameof(AirPortFactory),
                _ => string.Empty
            };
        }
    }
}

