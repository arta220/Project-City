namespace Domain.Map
{
    public readonly struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Position other) => X == other.X && Y == other.Y;

        public static bool operator ==(Position left, Position right) => left.Equals(right);
        public static bool operator !=(Position left, Position right) => !left.Equals(right);

        public override bool Equals(object obj) => obj is Position other && X == other.X && Y == other.Y;
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Возвращает соседние позиции по 4 направлениям (вверх, вниз, влево, вправо).
        /// </summary>
        public IEnumerable<Position> GetNeighbors()
        {
            yield return new Position(X, Y - 1); // вверх
            yield return new Position(X, Y + 1); // вниз
            yield return new Position(X - 1, Y); // влево
            yield return new Position(X + 1, Y); // вправо
        }

        /// <summary>
        /// Возвращает соседние позиции по 8 направлениям (включая диагонали).
        /// </summary>
        public IEnumerable<Position> GetNeighborsWithDiagonals()
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    yield return new Position(X + dx, Y + dy);
                }
            }
        }
    }
}
