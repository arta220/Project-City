using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;
using Domain.Map;

namespace CitySimulatorWPF.ViewModels
{
    public partial class BuildingIconVM : ObservableObject
    {
        /// <summary>
        /// Исходная доменная модель здания / объекта на карте.
        /// </summary>
        public MapObject MapObject { get; }

        /// <summary>
        /// Координаты верхнего левого угла иконки в пикселях (относительно Canvas).
        /// </summary>
        public double CanvasLeft { get; }
        public double CanvasTop { get; }

        /// <summary>
        /// Размер иконки в пикселях (ширина / высота = Area * tileSize).
        /// </summary>
        public double Width { get; }
        public double Height { get; }

        public BuildingIconVM(MapObject mapObject, Placement placement, int tileSize)
        {
            MapObject = mapObject;

            CanvasLeft = placement.Position.Y * tileSize;
            CanvasTop = placement.Position.X * tileSize;

            Width  = placement.Area.Height * tileSize;
            Height = placement.Area.Width  * tileSize;
        }
    }
}
