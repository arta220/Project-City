using CitySimulatorWPF.ViewModels;
using Domain.Base;
using Domain.Map;

namespace CitySimulatorWPF.Services
{
    /// <summary>
    /// Интерфейс сервиса строительства дорог.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Управляет процессом строительства дорог на карте.
    /// - Позволяет начать строительство, обновлять превью линии дороги и завершить строительство.
    /// - Поддерживает проверку текущих выбранных плиток и состояние строительства.
    ///
    /// Контекст использования:
    /// - Используется в MapVM для режима строительства дорог.
    /// - Взаимодействует с TileVM для визуального отображения превью и готовых плиток дороги.
    ///
    /// Расширяемость:
    /// - Можно добавить поддержку диагональных дорог, кривых линий или мультиплиточного здания.
    /// - Можно интегрировать проверку коллизий с другими объектами карты.
    /// </remarks>
    public interface IRoadConstructionService
    {
        /// <summary>
        /// Начинает строительство дороги с указанной плитки.
        /// </summary>
        void StartConstruction(TileVM startTile);

        /// <summary>
        /// Обновляет превью дороги по текущей позиции мыши.
        /// </summary>
        void UpdatePreview(TileVM currentTile);

        /// <summary>
        /// Завершает строительство дороги и вызывает callback для размещения объектов.
        /// </summary>
        void FinishConstruction(TileVM endTile, Func<Road, Placement, bool> placeRoadCallback);

        /// <summary>
        /// Очищает превью дороги.
        /// </summary>
        void ClearPreview(bool keepStartTile = false);

        /// <summary>
        /// Список текущих плиток, выбранных для строительства.
        /// </summary>
        IReadOnlyList<TileVM> CurrentTiles { get; }

        /// <summary>
        /// Указывает, ведется ли в данный момент строительство дороги.
        /// </summary>
        bool IsBuilding { get; }
    }

    /// <summary>
    /// Реализация сервиса строительства дорог.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Управляет визуальным превью строительства дороги.
    /// - Вычисляет прямую линию между стартовой и текущей плиткой.
    /// - Вызывает callback для окончательного размещения дороги на карте.
    ///
    /// Контекст использования:
    /// - Используется MapVM и TileVM для взаимодействия с UI.
    ///
    /// Расширяемость:
    /// - Можно добавить поддержку различных типов дорог (асфальт, гравий).
    /// - Можно добавлять ограничения по длине, пересечениям или ресурсам строительства.
    /// </remarks>
    public class RoadConstructionService : IRoadConstructionService
    {
        private readonly IReadOnlyList<TileVM> _allTiles;
        private TileVM _startTile;
        private List<TileVM> _tilesToBuild = new List<TileVM>();

        public RoadConstructionService(IReadOnlyList<TileVM> allTiles)
        {
            _allTiles = allTiles ?? throw new ArgumentNullException(nameof(allTiles));
        }

        public IReadOnlyList<TileVM> CurrentTiles => _tilesToBuild.AsReadOnly();
        public bool IsBuilding => _startTile != null;

        public void StartConstruction(TileVM startTile)
        {
            _startTile = startTile;
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

        public void FinishConstruction(TileVM endTile, Func<Road, Placement, bool> placeRoadCallback)
        {
            if (_startTile == null || _tilesToBuild.Count == 0) return;

            bool canBuildAll = _tilesToBuild.All(t => t.CanBuild);

            if (canBuildAll)
            {
                foreach (var tile in _tilesToBuild)
                {
                    var road = new Road(new Area(1, 1));
                    var placement = new Placement(new Domain.Map.Position(tile.X, tile.Y), road.Area);
                    placeRoadCallback?.Invoke(road, placement);
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
