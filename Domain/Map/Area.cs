namespace Domain.Map
{
    public readonly struct Area
    {
        public Position Position { get; }
        public int Width { get; }
        public int Height { get; }

        public Area(Position position, int width, int height)
            => (Position, Width, Height) = (position, width, height);

        public Area(int x, int y, int width, int height)
            : this(new Position(x, y), width, height) { }

        public Area(int width, int height) => (Width, Height) = (width, height);

        public int Left => Position.X;
        public int Top => Position.Y;
        public int Right => Position.X + Width;
        public int Bottom => Position.Y + Height;

        public bool Contains(Position point)
            => point.X >= Left && point.X < Right &&
               point.Y >= Top && point.Y < Bottom;

        public bool Intersects(Area other)
            => Left < other.Right && Right > other.Left &&
               Top < other.Bottom && Bottom > other.Top;

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
