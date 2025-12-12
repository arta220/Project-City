using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Common.Base;
using Domain.Base;
using Domain.Common.Enums;

namespace CitySimulatorWPF.Converters
{
    public class MapObjectToIconPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // debug
            System.Diagnostics.Debug.WriteLine($"[MapObjectToIconPath] value={value?.GetType().FullName ?? "null"}");
            
            if (value is not MapObject mapObject)
                return null;

            // TODO: подогнать тоже через enum
            // Жилые здания: маленький/высотный дом по количеству этажей
            if (mapObject is ResidentialBuilding residential)
            {
                return residential.Floors <= 2
                    ? "/Icons/SmallResidentialBuilding.png"
                    : "/Icons/HighResidentialBuilding.png";
            }

            // Коммерческие здания по типу
            if (mapObject is CommercialBuilding commercial)
            {
                return commercial.CommercialType switch
                {
                    Domain.Enums.CommercialType.Pharmacy => "/Icons/Pharmacy.png",
                    Domain.Enums.CommercialType.Shop => "/Icons/Shop.png",
                    Domain.Enums.CommercialType.Supermarket => "/Icons/Supermarket.png",
                    Domain.Enums.CommercialType.Cafe => "/Icons/Cafe.png",
                    Domain.Enums.CommercialType.Restaurant => "/Icons/Restaurant.png",
                    Domain.Enums.CommercialType.GasStation => "/Icons/GasStation.png",
                    _ => null
                };
            }

            // Промышленные здания по типу
            if (mapObject is IndustrialBuilding industrial)
            {
                // Определяем конкретный тип завода по цехам (проверки должны быть специфичными и идти от более специфичных к менее специфичным)
                
                // 1. Добывающий завод (ResourceExtractionFactory) - ТОЛЬКО производит Iron, Wood, Coal из None
                // Уникальный признак: все цеха используют None как входной материал И есть цех с Coal
                if (industrial.Workshops.Count > 0 && 
                    industrial.Workshops.All(w => w.InputMaterial.ToString() == "None") &&
                    industrial.Workshops.Any(w => w.OutputProduct.ToString() == "Coal"))
                {
                    return "/Icons/MiningFactory.png";
                }
                
                // 2. Древообрабатывающий завод (WoodProcessingFactory) - производит Lumber из Wood
                // Уникальный признак: есть цех, производящий Lumber (только у этого завода)
                if (industrial.Workshops.Any(w => w.OutputProduct.ToString() == "Lumber"))
                {
                    return "/Icons/WoodProcessingFactory.jpg";
                }
                
                // 3. Перерабатывающий завод (RecyclingFactory) - производит Steel из Iron
                // Уникальный признак: есть цех, производящий Steel из Iron (только у этого завода)
                if (industrial.Workshops.Any(w => 
                    w.InputMaterial.ToString() == "Iron" && w.OutputProduct.ToString() == "Steel"))
                {
                    return "/Icons/RecyclingFactory.png";
                }
                
                // 4. Остальные промышленные здания по типу (Factory или Warehouse)
                // Эти проверки идут последними, так как они менее специфичны
                return industrial.Type switch
                {
                    IndustrialBuildingType.Factory   => "/Icons/Factory.png",
                    IndustrialBuildingType.Warehouse => "/Icons/Warehouse.png",
                    _ => "/Icons/Factory.png" // По умолчанию Factory.png для неизвестных типов
                };
            }

            // Парки по типу
            if (mapObject is Park park)
            {
                return park.Type switch
                {
                    ParkType.UrbanPark        => "/Icons/UrbanPark.png",
                    ParkType.BotanicalGarden  => "/Icons/BotanGarden.png",
                    ParkType.Playground       => "/Icons/ChildPlayground.png",
                    ParkType.Square           => "/Icons/Square.png",
                    ParkType.RecreationArea   => "/Icons/RestArea.png",
                    _ => null
                };
            }

            // Дорога
            // if (mapObject is Road)
            //     return "/Icons/Road.png";

            // Для остальных типов пока иконку не задаём
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
