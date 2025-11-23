namespace CitySkylines_REMAKE.Models.Map
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
    }
}
