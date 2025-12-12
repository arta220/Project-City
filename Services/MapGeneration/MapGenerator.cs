using System;
using Domain.Common.Enums;
using Domain.Map;
using Services.Interfaces;

namespace Services.MapGenerator
{
    /// <summary>
    /// Генератор статичной карты для симуляции города.
    /// </summary>
    /// <remarks>
    /// Карта генерируется детерминированно (без Random) на основе координат.
    /// При одинаковых размерах карты результат всегда один и тот же.
    /// На карте присутствуют:
    /// - водная рамка по периметру;
    /// - диагональные полосы лугов;
    /// - несколько лесных массивов в виде эллипсов;
    /// - вытянутая горная цепь;
    /// - диагональная "змейка" реки;
    /// - распределение природных ресурсов (руды в горах, нефть и газ на равнинах и лугах).
    /// </remarks>
    public class MapGenerator : IMapGenerator
    {
        /// <summary>
        /// Генерирует новую карту указанного размера.
        /// </summary>
        /// <param name="width">Ширина карты в тайлах.</param>
        /// <param name="height">Высота карты в тайлах.</param>
        /// <returns>Сгенерированная модель карты <see cref="MapModel"/>.</returns>
        public MapModel GenerateMap(int width, int height)
        {
            var map = new MapModel(width, height);

            GenerateSimpleTerrain(map); // чисто для проверки работы всяких штук
           //GenerateTerrain(map);
           // GenerateResources(map);

            return map;
        }

        private void GenerateSimpleTerrain(MapModel map)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    map[x, y] = new TileModel() 
                    { 
                        Position = new Position(x, y),
                        Terrain = TerrainType.Plain
                    };
                }
            }
        }
        /// <summary>
        /// Инициализирует тайлы карты рельефом:
        /// водой, равнинами, лугами, лесом, горами и рекой.
        /// </summary>
        /// <param name="map">Карта, для которой нужно сгенерировать рельеф.</param>
        private void GenerateTerrain(MapModel map)
        {
            int width = map.Width;
            int height = map.Height;

            // Толщина водной рамки по периметру
            int border = 2;

            // Внутренняя область (суша)
            int innerWidth = width - 2 * border;
            int innerHeight = height - 2 * border;
            int innerArea = innerWidth * innerHeight;

            // ---- ЦЕЛЕВЫЕ ПЛОЩАДИ ----
            int targetMountainArea = innerArea * 6 / 100;   // ~6% горы
            int targetForestArea = innerArea * 10 / 100;    // ~10% лес

            // ================== ГОРЫ: ВЫТЯНУТАЯ ЦЕПЬ ==================
            // Толщина цепи по вертикали (максимум 4 клетки)
            int mountainThickness = Math.Min(4, innerHeight);

            // Длина цепи по горизонтали
            int mountainLength = targetMountainArea / mountainThickness;
            if (mountainLength > innerWidth)
                mountainLength = innerWidth;

            // Цепь внизу справа, вытянута по X
            int mountainStartX = border + innerWidth - mountainLength;
            int mountainStartY = border + innerHeight - mountainThickness;

            // ================== ЛЕС: 3 ЭЛЛИПСА ==================
            int ellipseCount = 3;
            double ellipseArea = (double)targetForestArea / ellipseCount;

            int forestRadiusX = Math.Max(3, innerWidth / 8);
            int forestRadiusY = Math.Max(3, (int)Math.Round(ellipseArea / (Math.PI * forestRadiusX)));
            if (forestRadiusY > innerHeight / 3)
                forestRadiusY = innerHeight / 3;

            // Центры лесных эллипсов: левый верх, правый верх, нижняя часть карты
            var forestCenters = new (int cx, int cy)[]
            {
                (border + innerWidth / 4,         border + innerHeight / 4),   // слева сверху
                (border + innerWidth * 3 / 4,     border + innerHeight / 3),   // справа сверху
                (border + innerWidth / 2,         border + innerHeight * 3 / 4)// нижняя часть
            };

            // Проверка, попадает ли точка (x, y) хотя бы в один лесной эллипс
            bool IsInForestEllipse(int x, int y)
            {
                foreach (var (cx, cy) in forestCenters)
                {
                    double dx = x - cx;
                    double dy = y - cy;

                    double value =
                        (dx * dx) / (double)(forestRadiusX * forestRadiusX) +
                        (dy * dy) / (double)(forestRadiusY * forestRadiusY);

                    if (value <= 1.0)
                        return true;
                }

                return false;
            }

            // ================== ЛУГА: ДИАГОНАЛЬНЫЕ ПОЛОСЫ ==================
            int stripeCount = 4;            // 3–5 полос, берём 4
            int stripeHalfWidth = 1;        // половина ширины полосы (~2–3 клетки общей ширины)

            // d = (x - y) для диагоналей; выберем несколько центров d
            int[] stripeCenters = new int[stripeCount];
            int minD = -innerHeight / 2;
            int maxD = innerWidth / 2;

            // Равномерно распределяем центры по диапазону [minD; maxD]
            for (int i = 0; i < stripeCount; i++)
            {
                double t = stripeCount == 1 ? 0.5 : (double)i / (stripeCount - 1); // 0..1
                stripeCenters[i] = (int)Math.Round(minD + t * (maxD - minD));
            }

            // Проверка принадлежности тайла к одной из диагональных полос луга
            bool IsInMeadowStripe(int x, int y)
            {
                // Считаем диагональ относительно внутренней области (без рамки)
                int localX = x - border;
                int localY = y - border;
                int d = localX - localY;

                foreach (int c in stripeCenters)
                {
                    if (Math.Abs(d - c) <= stripeHalfWidth)
                        return true;
                }

                return false;
            }

            // ================== РЕКА: ДИАГОНАЛЬНАЯ "ЗМЕЙКА" ==================
            // Половина ширины реки (в тайлах). Фактическая ширина ~3 тайла.
            int riverHalfWidth = 1;

            // ================== ГЕНЕРАЦИЯ ТАЙЛОВ ==================
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = new TileModel
                    {
                        Position = new Position(x, y),
                        ResourceType = NaturalResourceType.None,
                        ResourceAmount = 0
                    };

                    // 1) Водная рамка по периметру карты
                    if (x < border || y < border || x >= width - border || y >= height - border)
                    {
                        tile.Terrain = TerrainType.Water;
                        tile.Height = 0.0f;
                        map[x, y] = tile;
                        continue;
                    }

                    bool inMountainRect =
                        x >= mountainStartX && x < mountainStartX + mountainLength &&
                        y >= mountainStartY && y < mountainStartY + mountainThickness;

                    bool inForestEllipses = IsInForestEllipse(x, y);
                    bool inMeadowStripes = IsInMeadowStripe(x, y);

                    // 2) Приоритет: горы → лес → луга → равнина
                    if (inMountainRect)
                    {
                        tile.Terrain = TerrainType.Mountain;
                        tile.Height = 0.9f;
                    }
                    else if (inForestEllipses)
                    {
                        tile.Terrain = TerrainType.Forest;
                        tile.Height = 0.5f;
                    }
                    else if (inMeadowStripes)
                    {
                        tile.Terrain = TerrainType.Meadow;
                        tile.Height = 0.45f;
                    }
                    else
                    {
                        tile.Terrain = TerrainType.Plain;
                        tile.Height = 0.4f;
                    }

                    // 3) РЕКА ПЕРЕЗАТИРАЕТ ВСЁ: диагональная "змейка"
                    {
                        int localX = x - border;
                        int localY = y - border;

                        // Положение по вертикали [0..1] во внутренней области
                        double t = (double)localY / Math.Max(1, innerHeight - 1);

                        // Базовый центр реки по X:
                        // вверху ближе к левому краю (10% ширины),
                        // внизу — ближе к правому (60% ширины).
                        double riverCenterX = innerWidth * (0.1 + 0.5 * t);

                        // Небольшое "извивание" линии реки с помощью синуса
                        riverCenterX += Math.Sin(localY / 4.0) * 1.5;

                        // Если тайл попадает в полосу вокруг линии реки — делаем воду
                        if (Math.Abs(localX - riverCenterX) <= riverHalfWidth)
                        {
                            tile.Terrain = TerrainType.Water;
                            tile.Height = 0.1f;
                            tile.ResourceType = NaturalResourceType.None;
                            tile.ResourceAmount = 0;
                        }
                    }

                    map[x, y] = tile;
                }
            }
        }

        /// <summary>
        /// Распределяет ресурсы по уже сгенерированному рельефу.
        /// Железо/медь в горах, нефть/газ в равнинах и на лугах.
        /// </summary>
        /// <param name="map">Карта, на которой будут сгенерированы ресурсы.</param>
        private void GenerateResources(MapModel map)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var tile = map[x, y];

                    // Вода (в том числе река) – без ресурсов
                    if (tile.Terrain == TerrainType.Water)
                        continue;

                    // --- Руды в горах ---
                    if (tile.Terrain == TerrainType.Mountain)
                    {
                        // Небольшой узор через модуль суммы координат
                        int pattern = (x + y) % 3;

                        if (pattern == 0)
                        {
                            tile.ResourceType = NaturalResourceType.Iron;
                            tile.ResourceAmount = 150 + ((x * 13 + y * 7) % 100);
                        }
                        else if (pattern == 1)
                        {
                            tile.ResourceType = NaturalResourceType.Copper;
                            tile.ResourceAmount = 120 + ((x * 11 + y * 5) % 80);
                        }

                        continue;
                    }
                    if (tile.Terrain == TerrainType.Forest)
                    {
                        tile.ResourceType = NaturalResourceType.Wood;
                        tile.ResourceAmount = 100 + ((x * 7 + y * 5) % 80);
                    }

                    // --- Нефть и газ на равнинах и лугах ---
                    bool isFlat =
                        tile.Terrain == TerrainType.Plain ||
                        tile.Terrain == TerrainType.Meadow;

                    if (isFlat)
                    {
                        // Не размещаем ресурсы слишком близко к рамке
                        if (x < 4 || y < 4 || x > map.Width - 5 || y > map.Height - 5)
                            continue;

                        // "Узоры" – детерминированное распределение
                        if ((x + 2 * y) % 11 == 0)
                        {
                            tile.ResourceType = NaturalResourceType.Oil;
                            tile.ResourceAmount = 200 + ((x * 17 + y * 9) % 150);
                        }
                        else if ((2 * x + y) % 13 == 0)
                        {
                            tile.ResourceType = NaturalResourceType.Gas;
                            tile.ResourceAmount = 160 + ((x * 19 + y * 3) % 120);
                        }
                    }
                }
            }
        }
    }
}