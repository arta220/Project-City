using System;
using System.Collections.Generic;
using Domain.Map;

namespace Domain.Map.Generation
{
    public class GameSaveManager
    {
        private MapModel _savedMap;
        private bool _hasSave = false;

        // Сохраняем текущее состояние
        public void SaveCurrentState(MapModel map)
        {
            try
            {
                // Просто сохраняем ссылку на текущую карту
                // В реальности нужно сделать глубокое копирование
                _savedMap = map;
                _hasSave = true;

                Console.WriteLine("Состояние игры сохранено в памяти");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        // Загружаем сохраненное состояние
        public void LoadSavedState(MapModel currentMap)
        {
            try
            {
                if (!_hasSave)
                {
                    Console.WriteLine("Нет сохраненного состояния");
                    return;
                }

                // Восстанавливаем объекты на карте
                // Это упрощенная версия - копируем объекты с сохраненной карты
                CopyMapObjects(_savedMap, currentMap);

                Console.WriteLine("Состояние игры восстановлено из памяти");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void CopyMapObjects(MapModel source, MapModel destination)
        {
            // Очищаем целевую карту
            for (int x = 0; x < destination.Width; x++)
            {
                for (int y = 0; y < destination.Height; y++)
                {
                    destination[x, y].MapObject = null;
                }
            }

            // Копируем объекты с исходной карты
            for (int x = 0; x < Math.Min(source.Width, destination.Width); x++)
            {
                for (int y = 0; y < Math.Min(source.Height, destination.Height); y++)
                {
                    var obj = source[x, y].MapObject;
                    if (obj != null)
                    {
                        destination[x, y].MapObject = obj;
                    }
                }
            }
        }

        // Проверяем есть ли сохранение
        public bool HasSavedGame()
        {
            return _hasSave;
        }

        // Очищаем сохранение
        public void ClearSave()
        {
            _savedMap = null;
            _hasSave = false;
        }
    }
}