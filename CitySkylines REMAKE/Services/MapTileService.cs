using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CitySimulatorWPF.ViewModels;
using Domain.Map;
using System.Linq;

namespace CitySimulatorWPF.Services
{
    public interface IMapTileService
    {
        ObservableCollection<TileVM> Tiles { get; }
        void InitializeTiles(MapModel mapModel, Action<TileVM> onTileClicked, Action<TileVM> onTileConstructionStart, Func<TileVM, bool> onMouseOverPreview);
    }

    public class MapTileService : IMapTileService
    {
        public ObservableCollection<TileVM> Tiles { get; } = new ObservableCollection<TileVM>();

        public void InitializeTiles(MapModel mapModel, Action<TileVM> onTileClicked, Action<TileVM> onTileConstructionStart, Func<TileVM, bool> onMouseOverPreview)
        {
            Tiles.Clear();

            for (int x = 0; x < mapModel.Width; x++)
            {
                for (int y = 0; y < mapModel.Height; y++)
                {
                    var tileVM = new TileVM(mapModel[x, y]);

                    tileVM.TileClicked += (t) => onTileClicked?.Invoke(t);

                    tileVM.TileConstructionStart += (t) => onTileConstructionStart?.Invoke(t);

                    tileVM.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(tileVM.IsMouseOver) && tileVM.IsMouseOver)
                        {
                            onMouseOverPreview?.Invoke(tileVM);
                        }
                    };

                    Tiles.Add(tileVM);
                }
            }
        }
    }
}
