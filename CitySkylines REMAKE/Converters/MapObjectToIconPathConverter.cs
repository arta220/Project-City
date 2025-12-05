using System;
using System.Globalization;
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
                return industrial.Type switch
                {
                    IndustrialBuildingType.Factory   => "/Icons/Factory.png",
                    IndustrialBuildingType.Warehouse => "/Icons/Warehouse.png",
                    _ => null
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
