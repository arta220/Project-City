namespace Domain.Map
{
    /// <summary>
    /// Структура, описывающая прямоугольную область на карте.
    /// </summary>
    /// <remarks>
    /// Используется для определения размеров MapObject. 
    /// Может быть расширена дополнительными методами, например, для пересечений или проверки включения точки.
    /// </remarks>
    public readonly struct Area
    {
        /// <summary>
        /// Ширина области в клетках карты.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Высота области в клетках карты.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Создаёт область с указанными размерами.
        /// </summary>
        /// <param name="width">Ширина области в клетках.</param>
        /// <param name="height">Высота области в клетках.</param>
        public Area(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
