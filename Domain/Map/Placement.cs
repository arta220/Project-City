namespace Domain.Map
{
    public readonly struct Placement
    {
        public Position Position { get; }

        public Position Entrance { get; }
        public Area Area { get; }

        public Placement(Position position, Area area)
        {
            Position = position;
            Area = area;
            Entrance = new Position(
                position.X + area.Width / 2,
                position.Y + area.Height - 1
            );
        }
        public Placement(Position position, Position entrance, Area area)
            => (Position, Entrance, Area) = (position, entrance, area);
        public Placement(int x, int y, Area area)
            : this(new Position(x, y), area) { }

        public int Left => Position.X;

        public int Top => Position.Y;

        public int Right => Position.X + Area.Width;

        public int Bottom => Position.Y + Area.Height;

        public bool Contains(Position point)
            => point.X >= Left && point.X < Right &&
               point.Y >= Top && point.Y < Bottom;

        public bool Intersects(Placement other)
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
