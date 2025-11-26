namespace Domain.Map
{
    public readonly struct Area
    {
        public int Width { get; }
        public int Height { get; }

        public Area(int width, int height)
        {
            Width = width; 
            Height = height; 
        }
    }
}
