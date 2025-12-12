using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;
namespace Domain.Buildings
{
    /// <summary>
    /// Универсальный класс заводов
    /// Содержит общие свойства и методы для всех типов промышленных зданий
    /// Написание самих заводов происходит в <see cref="AllFactories"/>
    /// </summary>
    public class IndustrialBuilding : Building
    {
        public IndustrialBuildingType Type { get; }

        /// <summary>
        /// Внутренний класс — цех внутри завода.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Цех выполняет переработку одного типа ресурса в другой.
        /// Каждый цех имеет:
        /// - InputMaterial - входной материал (что перерабатывается)
        /// - OutputProduct - выходной продукт (что производится)
        /// - ProductionCoefficient - коэффициент производства (сколько единиц материала преобразуется)
        /// 
        /// ПРОЦЕСС РАБОТЫ:
        /// 1. Process() проверяет наличие InputMaterial в MaterialsBank или ProductsBank
        /// 2. Если материала достаточно, берет ProductionCoefficient единиц
        /// 3. Производит ProductionCoefficient единиц OutputProduct
        /// 4. Кладет продукт в ProductsBank
        /// 
        /// ОСОБЕННОСТИ:
        /// - Может брать материалы из MaterialsBank (входные материалы завода)
        /// - Может брать материалы из ProductsBank (продукты других цехов - цепочки производства)
        /// - Это позволяет создавать производственные цепочки: продукт одного цеха → материал другого
        /// 
        /// ПРИМЕР ЦЕПОЧКИ:
        /// Workshop 1: Wood → Lumber (Lumber попадает в ProductsBank)
        /// Workshop 2: Lumber → Furniture (берет Lumber из ProductsBank)
        /// </summary>
        public class Workshop
        {
            private readonly IndustrialBuilding _parent;

            /// <summary>
            /// Входной материал - что перерабатывается цехом.
            /// Может быть NaturalResourceType (природный ресурс) или ProductType (продукт другого цеха).
            /// </summary>
            public Enum InputMaterial { get; }
            
            /// <summary>
            /// Выходной продукт - что производится цехом.
            /// Может быть NaturalResourceType или ProductType.
            /// </summary>
            public Enum OutputProduct { get; }
            
            /// <summary>
            /// Коэффициент производства - количество единиц материала, которое преобразуется в продукт.
            /// 
            /// Пример: ProductionCoefficient = 8 означает:
            /// - Берет 8 единиц InputMaterial
            /// - Производит 8 единиц OutputProduct
            /// 
            /// Больший коэффициент = больше продукта за цикл = выше эффективность цеха.
            /// </summary>
            public int ProductionCoefficient { get; }

            /// <summary>
            /// Создает новый цех для завода.
            /// </summary>
            /// <param name="parent">Родительский завод, которому принадлежит цех</param>
            /// <param name="inputMaterial">Входной материал для переработки</param>
            /// <param name="outputProduct">Выходной продукт производства</param>
            /// <param name="productionCoefficient">Коэффициент производства (по умолчанию 1)</param>
            public Workshop(
                IndustrialBuilding parent,
                Enum inputMaterial,
                Enum outputProduct,
                int productionCoefficient = 1)
            {
                _parent = parent;
                InputMaterial = inputMaterial;
                OutputProduct = outputProduct;
                ProductionCoefficient = productionCoefficient;
            }

            /// <summary>
            /// Выполняет один цикл переработки ресурсов.
            /// 
            /// АЛГОРИТМ РАБОТЫ:
            /// 1. Проверяет наличие InputMaterial в MaterialsBank (входные материалы завода)
            /// 2. Если не найдено, проверяет ProductsBank (продукты других цехов - для цепочек)
            /// 3. Если материала достаточно (>= ProductionCoefficient):
            ///    - Уменьшает количество материала на ProductionCoefficient
            ///    - Увеличивает количество OutputProduct в ProductsBank на ProductionCoefficient
            /// 4. Возвращает true при успехе, false если материала недостаточно
            /// 
            /// ПРИМЕРЫ:
            /// - ResourceExtractionFactory: None (500) → Iron (8) - берет из MaterialsBank
            /// - WoodProcessingFactory: Lumber → Furniture - берет Lumber из ProductsBank (цепочка)
            /// 
            /// ПОТОК ДАННЫХ:
            /// ВХОД:  MaterialsBank[InputMaterial] или ProductsBank[InputMaterial]
            /// ПРОЦЕСС: Уменьшение материала, увеличение продукта
            /// ВЫХОД:  ProductsBank[OutputProduct] += ProductionCoefficient
            /// </summary>
            /// <returns>True, если переработка прошла успешно, иначе false (недостаточно материала)</returns>
            public bool Process()
            {
                // Шаг 1: Проверяем наличие входного материала
                int amount = 0;
                bool foundInMaterials = false;

                // Сначала проверяем MaterialsBank - хранилище входных материалов завода
                // Это основной источник материалов (например, Wood, Iron, Oil, None)
                if (_parent.MaterialsBank.TryGetValue(InputMaterial, out amount) && amount > 0)
                {
                    foundInMaterials = true;
                }
                // Если не найдено в MaterialsBank, проверяем ProductsBank
                // Это позволяет создавать цепочки производства:
                // продукт одного цеха (в ProductsBank) → материал другого цеха
                // Пример: Lumber (в ProductsBank) → Furniture
                else if (_parent.ProductsBank.TryGetValue(InputMaterial, out amount) && amount > 0)
                {
                    foundInMaterials = false;
                }
                else
                {
                    // Материал не найден ни в одном банке - цех не может работать
                    return false;
                }

                // Шаг 2: Проверяем, достаточно ли материала для производства
                if (amount < ProductionCoefficient)
                {
                    // Материала недостаточно - цех не может произвести продукт
                    return false;
                }

                // Шаг 3: Уменьшаем количество материала
                // Берем ProductionCoefficient единиц материала из соответствующего банка
                if (foundInMaterials)
                {
                    // Берем из MaterialsBank (входные материалы)
                    _parent.MaterialsBank[InputMaterial] = amount - ProductionCoefficient;
                }
                else
                {
                    // Берем из ProductsBank (продукт другого цеха - цепочка производства)
                    _parent.ProductsBank[InputMaterial] = amount - ProductionCoefficient;
                }

                // Шаг 4: Увеличиваем количество продукта
                // Производим ProductionCoefficient единиц продукта в ProductsBank
                if (!_parent.ProductsBank.ContainsKey(OutputProduct))
                {
                    // Инициализируем продукт, если его еще нет
                    _parent.ProductsBank[OutputProduct] = 0;
                }

                // Добавляем произведенный продукт
                _parent.ProductsBank[OutputProduct] += ProductionCoefficient;

                return true;
            }
        }

        /// <summary>
        /// Хранилище входных материалов завода.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Словарь, где ключ - тип материала (NaturalResourceType или ProductType),
        /// значение - количество единиц материала.
        /// 
        /// ИСПОЛЬЗОВАНИЕ:
        /// - Цехи берут материалы из MaterialsBank для производства
        /// - Материалы могут быть добавлены при создании завода (начальный запас)
        /// - Материалы могут поступать от других заводов (будущая функциональность)
        /// 
        /// ПРИМЕРЫ:
        /// - ResourceExtractionFactory: MaterialsBank[None] = 500
        /// - WoodProcessingFactory: MaterialsBank[Wood] = 300
        /// - RecyclingFactory: MaterialsBank[Iron] = 400, MaterialsBank[Oil] = 500
        /// </summary>
        public Dictionary<Enum, int> MaterialsBank = new Dictionary<Enum, int>();
        
        /// <summary>
        /// Хранилище произведенных продуктов завода.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Словарь, где ключ - тип продукта (NaturalResourceType или ProductType),
        /// значение - количество единиц продукта.
        /// 
        /// ИСПОЛЬЗОВАНИЕ:
        /// - Цехи кладут произведенные продукты в ProductsBank
        /// - Продукты могут использоваться другими цехами как материалы (цепочки производства)
        /// - Продукты могут экспортироваться или использоваться другими заводами
        /// 
        /// ПРИМЕРЫ:
        /// - ResourceExtractionFactory: ProductsBank[Iron] = 8, ProductsBank[Wood] = 12
        /// - WoodProcessingFactory: ProductsBank[Lumber] = 6, ProductsBank[Furniture] = 3
        /// - RecyclingFactory: ProductsBank[Steel] = 5, ProductsBank[Plastic] = 6
        /// 
        /// ОСОБЕННОСТЬ:
        /// ProductsBank может использоваться как источник материалов для цехов (цепочки):
        /// Workshop 1 производит Lumber → ProductsBank[Lumber] = 6
        /// Workshop 2 использует Lumber из ProductsBank → MaterialsBank не нужен
        /// </summary>
        public Dictionary<Enum, int> ProductsBank = new Dictionary<Enum, int>();
        
        /// <summary>
        /// Список всех цехов на заводе.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Каждый цех выполняет преобразование одного типа ресурса в другой.
        /// При вызове RunOnce() все цехи обрабатываются последовательно.
        /// 
        /// ПРИМЕРЫ:
        /// - ResourceExtractionFactory: 3 цеха (Iron, Wood, Coal)
        /// - WoodProcessingFactory: 4 цеха (Lumber, Furniture, Paper, WoodenCrate)
        /// - RecyclingFactory: 4 цеха (Steel, Plastic, Fuel, PlasticBottle)
        /// </summary>
        public List<Workshop> Workshops = new();

        public IndustrialBuilding(int floors, int maxOccupancy, Area area, IndustrialBuildingType type)
            : base(floors, maxOccupancy, area)
        {
            Type = type;
        }


        /// <summary>
        /// Добавляет новый цех на завод.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Создает Workshop, который будет преобразовывать input в output с коэффициентом coeff.
        /// 
        /// ПРИМЕРЫ:
        /// - AddWorkshop(None, Iron, 8) - цех добычи железа (None → Iron, коэф. 8)
        /// - AddWorkshop(Wood, Lumber, 6) - цех производства пиломатериалов (Wood → Lumber, коэф. 6)
        /// - AddWorkshop(Lumber, Furniture, 3) - цех производства мебели (Lumber → Furniture, коэф. 3)
        /// 
        /// ИСПОЛЬЗОВАНИЕ:
        /// Вызывается в Factory.Create() для настройки производственных цехов завода.
        /// </summary>
        /// <param name="input">Входной материал для переработки</param>
        /// <param name="output">Выходной продукт производства</param>
        /// <param name="coeff">Коэффициент производства (по умолчанию 1)</param>
        public void AddWorkshop(Enum input, Enum output, int coeff = 1)
        {
            // Создаем новый цех с указанными параметрами
            var ws = new Workshop(this, input, output, coeff);
            // Добавляем цех в список цехов завода
            Workshops.Add(ws);
        }

        /// <summary>
        /// Запускает один цикл производства для всех цехов завода.
        /// 
        /// КОНЦЕПЦИЯ:
        /// Вызывает Process() для каждого цеха последовательно.
        /// Каждый цех пытается произвести свой продукт, если есть достаточно материала.
        /// 
        /// ПРОЦЕСС:
        /// 1. Проходит по всем цехам в списке Workshops
        /// 2. Для каждого цеха вызывает Workshop.Process()
        /// 3. Process() проверяет наличие материала и производит продукт
        /// 
        /// ПОРЯДОК ОБРАБОТКИ:
        /// Цехи обрабатываются в порядке добавления (последовательно).
        /// Это важно для цепочек производства:
        /// - Если Workshop 1 производит Lumber, а Workshop 2 использует Lumber,
        ///   то Workshop 1 должен быть обработан раньше Workshop 2
        /// 
        /// ИСПОЛЬЗОВАНИЕ:
        /// - Вызывается вручную для тестирования
        /// - Вызывается автоматически IndustrialProductionService каждые 15 тиков
        /// 
        /// ПРИМЕР:
        /// ResourceExtractionFactory после RunOnce():
        /// - Workshop 1: None (500) → Iron (8) → MaterialsBank[None] = 492, ProductsBank[Iron] = 8
        /// - Workshop 2: None (492) → Wood (12) → MaterialsBank[None] = 480, ProductsBank[Wood] = 12
        /// - Workshop 3: None (480) → Coal (10) → MaterialsBank[None] = 470, ProductsBank[Coal] = 10
        /// </summary>
        public void RunOnce()
        {
            // Обрабатываем все цехи последовательно
            foreach (var ws in Workshops)
            {
                // Каждый цех пытается произвести свой продукт
                // Process() вернет false, если материала недостаточно
                ws.Process();
            }
        }
    }
}