using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CitySimulatorWPF.ViewModels;
using Domain.Map;
using System.Linq;

namespace CitySimulatorWPF.Services
{
    /// <summary>
    /// Интерфейс сервиса управления плитками карты.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Инициализирует и предоставляет коллекцию TileVM для UI.
    /// - Обеспечивает обработку кликов, начала строительства и наведения мыши.
    ///
    /// Контекст использования:
    /// - Используется MapVM для связывания плиток карты с визуальными элементами UI.
    /// - Взаимодействует с RoadConstructionService, MapVM и TileVM.
    ///
    /// Расширяемость:
    /// - Можно добавить фильтры плиток по типу (дороги, здания, вода и т.д.).
    /// - Можно реализовать виртуализацию для больших карт.
    /// </remarks>
    public interface IMapTileService
    {
        /// <summary>
        /// Коллекция всех плиток на карте.
        /// </summary>
        ObservableCollection<TileVM> Tiles { get; }

        /// <summary>
        /// Инициализирует плитки карты и подписывает события.
        /// </summary>
        /// <param name="mapModel">Модель карты, по которой создаются TileVM.</param>
        /// <param name="onTileClicked">Callback при клике на плитку.</param>
        /// <param name="onTileDoubleClicked">Callback при двойном клике на плитку.</param>
        /// <param name="onTileConstructionStart">Callback при начале строительства на плитке.</param>
        /// <param name="onMouseOverPreview">Callback при наведении мыши на плитку для превью строительства.</param>
        void InitializeTiles(
            MapModel mapModel,
            Action<TileVM> onTileClicked,
            Action<TileVM> onTileDoubleClicked,
            Action<TileVM> onTileConstructionStart,
            Func<TileVM, bool> onMouseOverPreview);
    }

    /// <summary>
    /// Реализация сервиса управления плитками карты.
    /// </summary>
    /// <remarks>
    /// Ответственность:
    /// - Создает TileVM для каждой позиции на карте.
    /// - Подписывает события TileClicked, TileConstructionStart и PropertyChanged для превью строительства.
    ///
    /// Контекст использования:
    /// - Используется MapVM для предоставления UI актуальных TileVM.
    /// - TileVM обрабатывает логику визуализации и взаимодействия пользователя с плитками.
    ///
    /// Расширяемость:
    /// - Можно добавить поддержку различных типов плиток (с разной проходимостью или графикой).
    /// </remarks>
    public class MapTileService : IMapTileService
    {
        public ObservableCollection<TileVM> Tiles { get; } = new ObservableCollection<TileVM>();

        public void InitializeTiles(MapModel mapModel, Action<TileVM> onTileClicked, Action<TileVM> onTileDoubleClicked, Action<TileVM> onTileConstructionStart, Func<TileVM, bool> onMouseOverPreview)
        {
            Tiles.Clear();

            for (int x = 0; x < mapModel.Width; x++)
            {
                for (int y = 0; y < mapModel.Height; y++)
                {
                    var tileVM = new TileVM(mapModel[x, y]);

                    tileVM.TileClicked += (t) => onTileClicked?.Invoke(t);
                    tileVM.TileDoubleClicked += (t) => onTileDoubleClicked?.Invoke(t);

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
