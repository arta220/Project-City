using System.Collections.Generic;

namespace Services.SaveLoad
{
    /// <summary>
    /// Структура данных для сохранения состояния игры.
    /// </summary>
    public class SaveData
    {
        /// <summary>
        /// Список сохраненных зданий.
        /// </summary>
        public List<BuildingSaveData> Buildings { get; set; } = new();

        /// <summary>
        /// Данные о размещении здания.
        /// </summary>
        public class BuildingSaveData
        {
            /// <summary>
            /// Тип здания (имя фабрики).
            /// </summary>
            public string BuildingType { get; set; } = string.Empty;

            /// <summary>
            /// Позиция X на карте.
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Позиция Y на карте.
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// Ширина здания.
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// Высота здания.
            /// </summary>
            public int Height { get; set; }
        }
    }
}

