# Концептуальное описание работы промышленных заводов

## Общая концепция системы производства

### Архитектура системы

```
┌─────────────────────────────────────────────────────────────┐
│                    IndustrialBuilding                       │
│  ┌──────────────────┐         ┌──────────────────┐        │
│  │  MaterialsBank   │         │   ProductsBank    │        │
│  │  (Входные        │         │   (Выходные       │        │
│  │   материалы)     │         │    продукты)      │        │
│  └──────────────────┘         └──────────────────┘        │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐ │
│  │              List<Workshop> Workshops                 │ │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐          │ │
│  │  │ Workshop │  │ Workshop │  │ Workshop │  ...      │ │
│  │  │   1      │  │   2      │  │   3      │          │ │
│  │  └──────────┘  └──────────┘  └──────────┘          │ │
│  └──────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### Ключевые компоненты

1. **IndustrialBuilding** - базовый класс завода
   - **Файл**: `Domain/Buildings/Industrial/IndustrialBuilding.cs`
   - **Содержит**: MaterialsBank, ProductsBank, Workshops
   - **Методы**: `AddWorkshop()`, `RunOnce()`

2. **Workshop** - внутренний класс цеха
   - **Файл**: `Domain/Buildings/Industrial/IndustrialBuilding.cs` (внутри класса)
   - **Свойства**: InputMaterial, OutputProduct, ProductionCoefficient
   - **Метод**: `Process()` - основная логика преобразования

3. **Factory** - фабрика для создания завода
   - **Файл**: `Domain/Factories/AllFactories.cs`
   - **Метод**: `Create()` - создает и настраивает завод

---

## 1. ДОБЫВАЮЩАЯ ПРОМЫШЛЕННОСТЬ (ResourceExtractionFactory)

### Концепция

**Назначение**: Первичное звено производственной цепочки. Добывает природные ресурсы из "ничего".

**Что производит**:
- Железо (Iron) - для металлургии
- Дерево (Wood) - для деревообработки  
- Уголь (Coal) - для энергетики

### Производственный процесс

```
┌─────────────────────────────────────────────────────────┐
│         ResourceExtractionFactory                       │
│                                                          │
│  MaterialsBank:                                         │
│    None = 500 (начальный запас)                         │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 1: None → Iron (коэф. 8)              │  │
│  │    Берет: 8 None из MaterialsBank                │  │
│  │    Производит: 8 Iron в ProductsBank             │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 2: None → Wood (коэф. 12)             │  │
│  │    Берет: 12 None из MaterialsBank               │  │
│  │    Производит: 12 Wood в ProductsBank            │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 3: None → Coal (коэф. 10)             │  │
│  │    Берет: 10 None из MaterialsBank               │  │
│  │    Производит: 10 Coal в ProductsBank            │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ProductsBank:                                          │
│    Iron, Wood, Coal (накапливаются)                    │
└─────────────────────────────────────────────────────────┘
```

### Методы для изучения

1. **ResourceExtractionFactory.Create()**
   - **Файл**: `Domain/Factories/AllFactories.cs:131-152`
   - **Что делает**: Создает IndustrialBuilding и настраивает его
   - **Ключевые действия**:
     ```csharp
     building.AddWorkshop(NaturalResourceType.None, NaturalResourceType.Iron, coeff: 8);
     building.AddWorkshop(NaturalResourceType.None, NaturalResourceType.Wood, coeff: 12);
     building.AddWorkshop(NaturalResourceType.None, NaturalResourceType.Coal, coeff: 10);
     building.MaterialsBank[NaturalResourceType.None] = 500;
     ```

2. **Workshop.Process()**
   - **Файл**: `Domain/Buildings/Industrial/IndustrialBuilding.cs:44-83`
   - **Что делает**: Преобразует входной материал в выходной продукт
   - **Алгоритм**:
     1. Проверяет наличие InputMaterial в MaterialsBank или ProductsBank
     2. Если есть достаточно материала:
        - Уменьшает MaterialsBank[InputMaterial] на ProductionCoefficient
        - Увеличивает ProductsBank[OutputProduct] на ProductionCoefficient
     3. Возвращает true при успехе, false при отсутствии материала

3. **IndustrialBuilding.RunOnce()**
   - **Файл**: `Domain/Buildings/Industrial/IndustrialBuilding.cs:109-113`
   - **Что делает**: Запускает Process() для всех цехов один раз
   - **Порядок**: Все цехи обрабатываются последовательно

### Поток данных

```
ВХОД:  MaterialsBank[None] = 500
       ↓
ПРОЦЕСС: RunOnce() → Workshop.Process() для каждого цеха
       ↓
ВЫХОД: ProductsBank[Iron] = 8
       ProductsBank[Wood] = 12
       ProductsBank[Coal] = 10
       MaterialsBank[None] = 500 - 30 = 470
```

### Использование в игре

Добытые ресурсы используются другими заводами:
- **Wood** → используется WoodProcessingFactory
- **Iron** → используется RecyclingFactory для производства стали
- **Coal** → используется для энергетики

---

## 2. ДЕРЕВООБРАБАТЫВАЮЩАЯ ПРОМЫШЛЕННОСТЬ (WoodProcessingFactory)

### Концепция

**Назначение**: Вторичное звено производственной цепочки. Перерабатывает дерево в различные продукты.

**Что производит**:
- Пиломатериалы (Lumber) - базовый продукт
- Мебель (Furniture) - из пиломатериалов (цепочка)
- Бумага (Paper) - напрямую из дерева
- Деревянные ящики (WoodenCrate) - напрямую из дерева

### Производственный процесс

```
┌─────────────────────────────────────────────────────────┐
│         WoodProcessingFactory                            │
│                                                          │
│  MaterialsBank:                                          │
│    Wood = 300 (начальный запас)                          │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 1: Wood → Lumber (коэф. 6)             │  │
│  │    Берет: 6 Wood из MaterialsBank                 │  │
│  │    Производит: 6 Lumber в ProductsBank           │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 2: Wood → Paper (коэф. 8)               │  │
│  │    Берет: 8 Wood из MaterialsBank                 │  │
│  │    Производит: 8 Paper в ProductsBank            │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 3: Wood → WoodenCrate (коэф. 5)        │  │
│  │    Берет: 5 Wood из MaterialsBank                 │  │
│  │    Производит: 5 WoodenCrate в ProductsBank      │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 4: Lumber → Furniture (коэф. 3)         │  │
│  │    Берет: 3 Lumber из ProductsBank ⚠️             │  │
│  │    Производит: 3 Furniture в ProductsBank        │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ProductsBank:                                          │
│    Lumber, Paper, WoodenCrate, Furniture                │
└─────────────────────────────────────────────────────────┘
```

### Особенности

**Цепочка производства**: 
- Wood → Lumber → Furniture (двухэтапный процесс)
- Workshop 4 использует Lumber из ProductsBank как материал

**Параллельное производство**:
- Из одного Wood можно сделать Paper, Lumber или WoodenCrate
- Все три цеха работают независимо

### Методы для изучения

1. **WoodProcessingFactory.Create()**
   - **Файл**: `Domain/Factories/AllFactories.cs:157-179`
   - **Ключевые действия**:
     ```csharp
     building.AddWorkshop(NaturalResourceType.Wood, ProductType.Lumber, coeff: 6);
     building.AddWorkshop(ProductType.Lumber, ProductType.Furniture, coeff: 3); // Цепочка!
     building.AddWorkshop(NaturalResourceType.Wood, ProductType.Paper, coeff: 8);
     building.AddWorkshop(NaturalResourceType.Wood, ProductType.WoodenCrate, coeff: 5);
     building.MaterialsBank[NaturalResourceType.Wood] = 300;
     ```

2. **Workshop.Process() - работа с цепочками**
   - **Важно**: Process() может брать материалы из ProductsBank
   - **Логика**: 
     ```csharp
     // Сначала проверяет MaterialsBank
     if (MaterialsBank.TryGetValue(InputMaterial, out amount))
     // Если не найдено, проверяет ProductsBank
     else if (ProductsBank.TryGetValue(InputMaterial, out amount))
     ```
   - Это позволяет создавать цепочки: продукт одного цеха → материал другого цеха

### Поток данных

```
ВХОД:  MaterialsBank[Wood] = 300
       ↓
ПРОЦЕСС ЦИКЛ 1: RunOnce()
       ├─ Workshop 1: Wood → Lumber (6 Wood → 6 Lumber)
       ├─ Workshop 2: Wood → Paper (8 Wood → 8 Paper)
       ├─ Workshop 3: Wood → WoodenCrate (5 Wood → 5 WoodenCrate)
       └─ Workshop 4: Lumber → Furniture (3 Lumber → 3 Furniture) ⚠️
       ↓
ВЫХОД: ProductsBank[Lumber] = 6
       ProductsBank[Paper] = 8
       ProductsBank[WoodenCrate] = 5
       ProductsBank[Furniture] = 3
       MaterialsBank[Wood] = 300 - 19 = 281
```

**Важно**: Workshop 4 может работать только если Workshop 1 уже произвел Lumber!

---

## 3. ПЕРЕРАБАТЫВАЮЩАЯ ПРОМЫШЛЕННОСТЬ (RecyclingFactory)

### Концепция

**Назначение**: Вторичное звено производственной цепочки. Перерабатывает базовые ресурсы в сложные продукты.

**Что производит**:
- Сталь (Steel) - из железа
- Пластик (Plastic) - из нефти
- Топливо (Fuel) - из нефти
- Пластиковые бутылки (PlasticBottle) - из пластика (цепочка)

### Производственный процесс

```
┌─────────────────────────────────────────────────────────┐
│         RecyclingFactory                                 │
│                                                          │
│  MaterialsBank:                                          │
│    Iron = 400 (начальный запас)                          │
│    Oil = 500 (начальный запас)                           │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 1: Iron → Steel (коэф. 5)              │  │
│  │    Берет: 5 Iron из MaterialsBank                │  │
│  │    Производит: 5 Steel в ProductsBank             │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 2: Oil → Plastic (коэф. 6)             │  │
│  │    Берет: 6 Oil из MaterialsBank                  │  │
│  │    Производит: 6 Plastic в ProductsBank          │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 3: Oil → Fuel (коэф. 8)                 │  │
│  │    Берет: 8 Oil из MaterialsBank                  │  │
│  │    Производит: 8 Fuel в ProductsBank              │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │  Workshop 4: Plastic → PlasticBottle (коэф. 10)  │  │
│  │    Берет: 10 Plastic из ProductsBank ⚠️          │  │
│  │    Производит: 10 PlasticBottle в ProductsBank   │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ProductsBank:                                          │
│    Steel, Plastic, Fuel, PlasticBottle                  │
└─────────────────────────────────────────────────────────┘
```

### Особенности

**Два типа входных материалов**:
- Iron (400) - для производства стали
- Oil (500) - для производства пластика и топлива

**Цепочка производства**:
- Oil → Plastic → PlasticBottle (двухэтапный процесс)
- Workshop 4 использует Plastic из ProductsBank

**Параллельное производство**:
- Из одного Oil можно сделать Plastic или Fuel
- Оба цеха работают независимо

### Методы для изучения

1. **RecyclingFactory.Create()**
   - **Файл**: `Domain/Factories/AllFactories.cs:184-207`
   - **Ключевые действия**:
     ```csharp
     building.AddWorkshop(NaturalResourceType.Iron, ProductType.Steel, coeff: 5);
     building.AddWorkshop(NaturalResourceType.Oil, ProductType.Plastic, coeff: 6);
     building.AddWorkshop(NaturalResourceType.Oil, ProductType.Fuel, coeff: 8);
     building.AddWorkshop(ProductType.Plastic, ProductType.PlasticBottle, coeff: 10); // Цепочка!
     building.MaterialsBank[NaturalResourceType.Iron] = 400;
     building.MaterialsBank[NaturalResourceType.Oil] = 500;
     ```

### Поток данных

```
ВХОД:  MaterialsBank[Iron] = 400
       MaterialsBank[Oil] = 500
       ↓
ПРОЦЕСС ЦИКЛ 1: RunOnce()
       ├─ Workshop 1: Iron → Steel (5 Iron → 5 Steel)
       ├─ Workshop 2: Oil → Plastic (6 Oil → 6 Plastic)
       ├─ Workshop 3: Oil → Fuel (8 Oil → 8 Fuel)
       └─ Workshop 4: Plastic → PlasticBottle (10 Plastic → 10 PlasticBottle) ⚠️
       ↓
ВЫХОД: ProductsBank[Steel] = 5
       ProductsBank[Plastic] = 6
       ProductsBank[Fuel] = 8
       ProductsBank[PlasticBottle] = 10
       MaterialsBank[Iron] = 400 - 5 = 395
       MaterialsBank[Oil] = 500 - 14 = 486
```

**Важно**: Workshop 4 может работать только если Workshop 2 уже произвел Plastic!

---

## Общие принципы работы системы

### 1. Цикл производства

```
┌─────────────┐
│  RunOnce()  │
└──────┬──────┘
       │
       ├─→ Workshop 1.Process()
       │   ├─ Проверка MaterialsBank[InputMaterial]
       │   ├─ Уменьшение MaterialsBank
       │   └─ Увеличение ProductsBank[OutputProduct]
       │
       ├─→ Workshop 2.Process()
       │   ├─ Проверка MaterialsBank или ProductsBank[InputMaterial]
       │   ├─ Уменьшение MaterialsBank или ProductsBank
       │   └─ Увеличение ProductsBank[OutputProduct]
       │
       └─→ Workshop N.Process()
           └─ ... (то же самое)
```

### 2. Цепочки производства

**Как это работает**:
1. Workshop A производит Product X в ProductsBank
2. Workshop B использует Product X как InputMaterial
3. Workshop.Process() проверяет ProductsBank, если не находит в MaterialsBank
4. Workshop B берет Product X из ProductsBank и производит Product Y

**Пример**:
```
Workshop 1: Wood → Lumber (Lumber попадает в ProductsBank)
Workshop 2: Lumber → Furniture (берет Lumber из ProductsBank)
```

### 3. Параллельное производство

Несколько цехов могут использовать один и тот же материал:
```
Workshop 1: Oil → Plastic
Workshop 2: Oil → Fuel
```
Оба цеха конкурируют за один и тот же ресурс (Oil).

### 4. Коэффициент производства (ProductionCoefficient)

**Что это**: Количество единиц материала, которое преобразуется в столько же единиц продукта.

**Пример**:
- ProductionCoefficient = 8 означает: берет 8 единиц материала → производит 8 единиц продукта
- Больший коэффициент = больше продукта за цикл = выше эффективность

---

## Ключевые файлы для изучения

### Основные классы

1. **IndustrialBuilding.cs**
   - Путь: `Domain/Buildings/Industrial/IndustrialBuilding.cs`
   - Содержит: класс Workshop, методы RunOnce(), AddWorkshop()
   - **Изучить**: Workshop.Process() - основная логика преобразования

2. **AllFactories.cs**
   - Путь: `Domain/Factories/AllFactories.cs`
   - Содержит: ResourceExtractionFactory, WoodProcessingFactory, RecyclingFactory
   - **Изучить**: метод Create() каждой фабрики - как настраиваются заводы

### Сервисы

3. **IndustrialProductionService.cs**
   - Путь: `Services/IndustrialProduction/IndustrialProductionService.cs`
   - **Что делает**: Автоматически вызывает RunOnce() для всех заводов каждые 15 тиков
   - **Изучить**: метод Update() - как интегрируется в игровой цикл

---

## Резюме концепции

### Добывающая промышленность
- **Вход**: None (500)
- **Выход**: Iron, Wood, Coal
- **Особенность**: Первичное звено, добывает из "ничего"

### Деревообрабатывающая промышленность
- **Вход**: Wood (300)
- **Выход**: Lumber, Paper, WoodenCrate, Furniture
- **Особенность**: Цепочка производства (Wood → Lumber → Furniture)

### Перерабатывающая промышленность
- **Вход**: Iron (400), Oil (500)
- **Выход**: Steel, Plastic, Fuel, PlasticBottle
- **Особенность**: Два типа входных материалов, цепочка (Oil → Plastic → PlasticBottle)

### Общее
- Все заводы используют одну и ту же систему: MaterialsBank → Workshop.Process() → ProductsBank
- Цепочки производства возможны через использование ProductsBank как источника материалов
- ProductionCoefficient определяет эффективность каждого цеха
