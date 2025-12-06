# Тестовый сценарий для заводов картона и упаковки

## Описание

Этот набор тестов демонстрирует работу двух новых заводов:
- **Завод по производству картона** (CardboardFactory)
- **Завод по производству упаковки** (PackagingFactory)

## Запуск тестов

Для запуска тестов используйте команду:
```bash
dotnet test
```

Или через Visual Studio: Test Explorer -> Run All Tests

## Что проверяют тесты

### CardboardFactoryTests

1. **CardboardFactory_CreatesBuilding_WithCorrectProperties**
   - Проверяет, что завод создается с правильными параметрами (2 этажа, 12 рабочих мест, размер 5x5)

2. **CardboardFactory_CreatesBuilding_WithWorkshops**
   - Проверяет, что завод имеет цеха (минимум 7 цехов)

3. **CardboardFactory_HasInitialMaterials**
   - Проверяет наличие начальных материалов:
     - Древесная щепа (WoodChips)
     - Макулатура (RecycledPaper)
     - Химикаты (Chemicals)
     - Вода (Water)
     - Энергия (Energy)

4. **CardboardFactory_Production_ProducesCardboardProducts**
   - Проверяет, что после запуска производства появляются продукты картона

5. **CardboardFactory_Workshop_ProcessesMaterials**
   - Проверяет, что материалы расходуются при производстве

### PackagingFactoryTests

1. **PackagingFactory_CreatesBuilding_WithCorrectProperties**
   - Проверяет, что завод создается с правильными параметрами (2 этажа, 15 рабочих мест, размер 6x6)

2. **PackagingFactory_CreatesBuilding_WithWorkshops**
   - Проверяет, что завод имеет цеха (минимум 10 цехов)

3. **PackagingFactory_HasInitialMaterials**
   - Проверяет наличие начальных материалов для производства упаковки

4. **PackagingFactory_Production_ProducesPackagingProducts**
   - Проверяет, что после запуска производства появляются продукты упаковки

## Демонстрация работы

Тесты демонстрируют полный цикл работы заводов:

1. **Создание заводов** через фабрики
2. **Инициализация** с начальными материалами
3. **Производство** продукции через цеха
4. **Использование материалов** при производстве
5. **Накопление продуктов** в ProductsBank

## Ожидаемые результаты

После запуска всех тестов вы должны увидеть:
- ✅ Все тесты проходят успешно
- ✅ Заводы создаются корректно
- ✅ Производство работает и создает продукты
- ✅ Материалы расходуются при производстве

