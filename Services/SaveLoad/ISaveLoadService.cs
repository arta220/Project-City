using Domain.Common.Base;
using Domain.Map;

namespace Services.SaveLoad
{
    /// <summary>
    /// Интерфейс сервиса сохранения и загрузки игры.
    /// </summary>
    public interface ISaveLoadService
    {
        /// <summary>
        /// Сохраняет состояние игры в файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу сохранения.</param>
        /// <param name="simulation">Экземпляр симуляции для сохранения.</param>
        /// <returns>True, если сохранение успешно, иначе false.</returns>
        bool SaveGame(string filePath, Simulation simulation);

        /// <summary>
        /// Загружает состояние игры из файла.
        /// </summary>
        /// <param name="filePath">Путь к файлу сохранения.</param>
        /// <param name="simulation">Экземпляр симуляции для загрузки.</param>
        /// <returns>True, если загрузка успешна, иначе false.</returns>
        bool LoadGame(string filePath, Simulation simulation);
    }
}

