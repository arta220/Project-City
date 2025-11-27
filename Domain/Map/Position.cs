namespace Domain.Map
{
    /// <summary>
    /// Представляет позицию на карте.
    /// Используется для тайлов, зданий, граждан и других объектов.
    /// </summary>
    public readonly struct Position
    {
        /// <summary>
        /// Координата X на карте.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Координата Y на карте.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Создаёт новую позицию.
        /// </summary>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Проверяет равенство двух позиций.
        /// </summary>
        public bool Equals(Position other) => X == other.X && Y == other.Y;

        public static bool operator ==(Position left, Position right) => left.Equals(right);
        public static bool operator !=(Position left, Position right) => !left.Equals(right);

        public override bool Equals(object obj) => obj is Position other && X == other.X && Y == other.Y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
