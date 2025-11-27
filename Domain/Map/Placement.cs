namespace Domain.Map
{
    /// <summary>
    /// Представляет область на карте с конкретной позицией и размерами.
    /// Используется для размещения зданий, дорог и других объектов на карте.
    /// </summary>
    public readonly struct Placement
    {
        /// <summary>
        /// Верхний левый угол области.
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// Размер области.
        /// </summary>
        public Area Area { get; }

        /// <summary>
        /// Создаёт новый Placement с указанной позицией и размерами.
        /// </summary>
        public Placement(Position position, Area area)
            => (Position, Area) = (position, area);

        /// <summary>
        /// Создаёт Placement по координатам X, Y и размеру.
        /// </summary>
        public Placement(int x, int y, Area area)
            : this(new Position(x, y), area) { }

        /// <summary>
        /// Левая граница области.
        /// </summary>
        public int Left => Position.X;

        /// <summary>
        /// Верхняя граница области.
        /// </summary>
        public int Top => Position.Y;

        /// <summary>
        /// Правая граница области (не включительно).
        /// </summary>
        public int Right => Position.X + Area.Width;

        /// <summary>
        /// Нижняя граница области (не включительно).
        /// </summary>
        public int Bottom => Position.Y + Area.Height;

        /// <summary>
        /// Проверяет, находится ли точка внутри этой области.
        /// </summary>
        public bool Contains(Position point)
            => point.X >= Left && point.X < Right &&
               point.Y >= Top && point.Y < Bottom;

        /// <summary>
        /// Проверяет, пересекается ли эта область с другой.
        /// </summary>
        public bool Intersects(Placement other)
            => Left < other.Right && Right > other.Left &&
               Top < other.Bottom && Bottom > other.Top;

        /// <summary>
        /// Возвращает все позиции, покрываемые этой областью.
        /// Полезно для инициализации тайлов карты при размещении зданий или дорог.
        /// </summary>
        public IEnumerable<Position> GetAllPositions()
        {
            for (int x = Left; x < Right; x++)
            {
                for (int y = Top; y < Bottom; y++)
                {
                    yield return new Position(x, y);
                }
            }
        }
    }
}
