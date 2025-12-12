# Тестовый сценарий для системы сохранения и загрузки

## Описание

Этот набор тестов демонстрирует работу системы сохранения и загрузки игры:
- **SaveLoadService** - основной сервис сохранения и загрузки
- **SaveData** - структура данных для сохранения состояния игры

## Запуск тестов

Для запуска тестов используйте команду:
```bash
dotnet test
```

Или через Visual Studio: Test Explorer -> Run All Tests

## Что проверяют тесты

### SaveLoadServiceTests

1. **SaveLoadService_SaveGame_WithWoodProcessingFactory_ReturnsTrue**
   - Проверяет сохранение игры с древообрабатывающим заводом
   - Проверяет создание файла сохранения

2. **SaveLoadService_SaveGame_WithRecyclingFactory_ReturnsTrue**
   - Проверяет сохранение игры с перерабатывающим заводом
   - Проверяет создание файла сохранения

3. **SaveLoadService_SaveGame_WithMultipleBuildings_SavesAll**
   - Проверяет сохранение нескольких зданий одновременно
   - Проверяет корректность сохранения всех типов зданий

4. **SaveLoadService_LoadGame_WithNonExistentFile_ReturnsFalse**
   - Проверяет обработку несуществующего файла
   - Проверяет возврат false при отсутствии файла

5. **SaveLoadService_LoadGame_WithValidFile_LoadsBuildings**
   - Проверяет загрузку зданий из файла
   - Проверяет корректность восстановления состояния

6. **SaveLoadService_SaveAndLoad_WithWoodProcessingFactory_PreservesProperties**
   - Проверяет сохранение всех свойств древообрабатывающего завода
   - Проверяет корректность восстановления размеров и характеристик

7. **SaveLoadService_SaveAndLoad_WithRecyclingFactory_PreservesWorkshops**
   - Проверяет сохранение цехов перерабатывающего завода
   - Проверяет корректность восстановления производственных цехов

### SaveDataTests

1. **SaveData_Initialization_CreatesEmptyBuildingsList**
   - Проверяет инициализацию пустого списка зданий

2. **SaveData_BuildingSaveData_Initialization_SetsDefaultValues**
   - Проверяет значения по умолчанию для данных здания

3. **SaveData_BuildingSaveData_CanSetProperties**
   - Проверяет установку свойств для древообрабатывающего завода

4. **SaveData_BuildingSaveData_CanSetRecyclingFactory**
   - Проверяет установку свойств для перерабатывающего завода

5. **SaveData_CanAddMultipleBuildings**
   - Проверяет добавление нескольких зданий в структуру данных

### SaveLoadIntegrationTests

1. **SaveLoadIntegration_SaveAndLoadWoodProcessingFactory_CompleteCycle**
   - Проверяет полный цикл сохранения и загрузки древообрабатывающего завода
   - Проверяет сохранение цехов и материалов

2. **SaveLoadIntegration_SaveAndLoadRecyclingFactory_CompleteCycle**
   - Проверяет полный цикл сохранения и загрузки перерабатывающего завода
   - Проверяет сохранение всех производственных цехов

3. **SaveLoadIntegration_SaveAndLoadMultipleFactories_PreservesAll**
   - Проверяет сохранение и загрузку нескольких заводов одновременно
   - Проверяет корректность восстановления всех типов зданий

4. **SaveLoadIntegration_SaveAndLoad_PreservesPlacement**
   - Проверяет сохранение позиций зданий на карте
   - Проверяет корректность восстановления размещения

## Демонстрация работы

Тесты демонстрируют полный цикл работы системы сохранения и загрузки:

1. **Создание зданий** через фабрики
2. **Размещение зданий** на карте
3. **Сохранение состояния** в JSON файл
4. **Очистка карты** перед загрузкой
5. **Загрузка состояния** из файла
6. **Восстановление зданий** с сохранением всех свойств

## Ожидаемые результаты

После запуска всех тестов вы должны увидеть:
- ✅ Все тесты проходят успешно
- ✅ Файлы сохранения создаются корректно
- ✅ Здания сохраняются и загружаются с сохранением всех свойств
- ✅ Цеха и материалы сохраняются для промышленных зданий
- ✅ Позиции зданий на карте сохраняются точно

