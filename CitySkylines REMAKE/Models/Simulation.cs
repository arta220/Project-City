namespace CitySkylines_REMAKE.Models
{
    public class Simulation
    {
        private const int DEFAULT_MAP_SIZE = 25;
        public MapModel MapModel { get; private set; }
        public Simulation()
        {
            InitializeSimulation();
        }
        
        private void InitializeSimulation()
        {
            MapModel = new(DEFAULT_MAP_SIZE, DEFAULT_MAP_SIZE);
            InitializeMapTiles();
        }
        private void InitializeMapTiles()
        {
            for (int x = 0; x < MapModel.Width; x++)
            {
                for (int y = 0; y < MapModel.Height; y++)
                {
                    MapModel[x, y] = new TileModel();
                }
            }
        }
    }
}
