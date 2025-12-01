using CitySimulatorWPF.ViewModels;
using Domain.Common.Enums;
using Domain.Enums;
using Domain.Infrastructure;
using Domain.Map;
using Services.Interfaces;

namespace CitySimulatorWPF.Services
{
    public class PathConstructionService : IPathConstructionService
    {
        private readonly IReadOnlyList<TileVM> _allTiles;
        private TileVM _startTile;
        private List<TileVM> _tilesToBuild = new List<TileVM>();
        private PathType _currentPathType;

        public PathConstructionService(IReadOnlyList<TileVM> allTiles)
        {
            _allTiles = allTiles ?? throw new ArgumentNullException(nameof(allTiles));
        }

        public IReadOnlyList<TileVM> CurrentTiles => _tilesToBuild.AsReadOnly();
        public bool IsBuilding => _startTile != null;
        public PathType CurrentPathType => _currentPathType;

        public void StartConstruction(TileVM startTile, PathType pathType)
        {
            _startTile = startTile;
            _currentPathType = pathType;
            ClearPreview();
            _tilesToBuild.Add(_startTile);
            _startTile.IsPreviewTile = true;
        }

        public void UpdatePreview(TileVM currentTile)
        {
            if (_startTile == null || currentTile == _startTile) return;

            ClearPreview(keepStartTile: true);
            _tilesToBuild = GetTilesAlongLine(_startTile, currentTile);

            foreach (var tile in _tilesToBuild)
            {
                if (tile.CanBuild)
                {
                    tile.IsPreviewTile = true;
                }
                else
                {
                    ClearPreview(keepStartTile: true);
                    break;
                }
            }
        }

        public void FinishConstruction(TileVM endTile, Func<Path, Placement, bool> placePathCallback)
        {
            if (_startTile == null || _tilesToBuild.Count == 0) return;

            bool canBuildAll = _tilesToBuild.All(t => t.CanBuild);

            if (canBuildAll)
            {
                foreach (var tile in _tilesToBuild)
                {
                    Path path = _currentPathType == PathType.Pedestrian
                        ? new PedestrianPath()
                        : new BicyclePath();

                    var placement = new Placement(new Position(tile.X, tile.Y), path.Area);
                    placePathCallback?.Invoke(path, placement);
                }
            }

            ClearPreview();
            _startTile = null;
            _tilesToBuild.Clear();
        }

        public void ClearPreview(bool keepStartTile = false)
        {
            foreach (var tile in _tilesToBuild)
            {
                if (keepStartTile && tile == _startTile) continue;
                tile.IsPreviewTile = false;
            }

            _tilesToBuild.Clear();
            if (keepStartTile && _startTile != null)
                _tilesToBuild.Add(_startTile);
        }

        private List<TileVM> GetTilesAlongLine(TileVM start, TileVM end)
        {
            var lineTiles = new List<TileVM>();

            int x0 = start.X;
            int y0 = start.Y;
            int x1 = end.X;
            int y1 = end.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                TileVM currentTile = _allTiles.FirstOrDefault(t => t.X == x0 && t.Y == y0);
                if (currentTile != null)
                    lineTiles.Add(currentTile);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return lineTiles;
        }
    }
}