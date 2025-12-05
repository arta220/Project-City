using System;
using System.Globalization;
using System.Windows.Data;
using Domain.Buildings;
using Domain.Buildings.Residential;
using Domain.Common.Base;

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

            if (mapObject is ResidentialBuilding)
                return "/Icons/HighResidentialBuilding.png";

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

            // Временный fallback: для любых других объектов показываем одну тестовую иконку,
            // чтобы убедиться, что биндинг и конвертер вообще работают.
            return "/Icons/Shop.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
