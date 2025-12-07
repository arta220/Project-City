using CitySimulatorWPF.ViewModels;
using Domain.Enums;
using Domain.Map;
using Domain.Infrastructure;
using Domain.Common.Enums;

namespace CitySimulatorWPF.Services
{
    public interface IPathConstructionService
    {
        void StartConstruction(TileVM startTile, PathType pathType);
        void UpdatePreview(TileVM currentTile);
        void FinishConstruction(TileVM endTile, Func<Path, Placement, bool> placePathCallback);
        void ClearPreview(bool keepStartTile = false);
        IReadOnlyList<TileVM> CurrentTiles { get; } 
        bool IsBuilding { get; }
        PathType CurrentPathType { get; }
    }
}