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

            // Position.X is horizontal (column), Position.Y is vertical (row)
            CanvasLeft = placement.Position.X * tileSize;
            CanvasTop = placement.Position.Y * tileSize;

            // Area.Width is horizontal size, Area.Height is vertical size
            Width  = placement.Area.Width * tileSize;
            Height = placement.Area.Height * tileSize;
        }
    }
}
