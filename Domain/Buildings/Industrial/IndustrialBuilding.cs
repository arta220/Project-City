using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;
using System.Security.AccessControl;
using static Domain.Buildings.IndustrialBuilding;

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
        /// Выполняет переработку одного типа ресурса в другой.
        /// </summary>
        public class Workshop
        {
            private readonly IndustrialBuilding _parent;

            public Enum InputMaterial { get; }
            public Enum OutputProduct { get; }
            public int ProductionCoefficient { get; }

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
            /// Выполняет переработку ресурсов:
            /// забирает ProductionCoefficient материала и производит ProductionCoefficient единиц продукции.
            /// </summary>
            /// <returns> True, если переработка прошла успешно, иначе false </returns>
            public bool Process()
            {
                // Проверяем, есть ли входной материал
                int amount = 0;
                bool foundInMaterials = false;

                // Сначала проверка MaterialsBank
                if (_parent.MaterialsBank.TryGetValue(InputMaterial, out amount) && amount > 0)
                {
                    foundInMaterials = true;
                }
                // Если не найдено в MaterialsBank, проверка ProductsBank (продукты могут быть использованы как материалы)
                // TODO: сомнительное обращение к списку продукции завода как к списку сырья. скорее всего удалить в будущем
                else if (_parent.ProductsBank.TryGetValue(InputMaterial, out amount) && amount > 0)
                {
                    foundInMaterials = false;
                }
                else
                {
                    return false; // Материал не найден ни в одном банке
                }

                // Уменьшение количества материала
                if (foundInMaterials)
                {
                    _parent.MaterialsBank[InputMaterial] = amount - ProductionCoefficient;
                }
                else
                {
                    _parent.ProductsBank[InputMaterial] = amount - ProductionCoefficient;
                }

                // Увеличение количества продукта
                if (!_parent.ProductsBank.ContainsKey(OutputProduct))
                    _parent.ProductsBank[OutputProduct] = 0;

                _parent.ProductsBank[OutputProduct] += ProductionCoefficient;

                return true;
            }
        }

        public Dictionary<Enum, int> MaterialsBank = new Dictionary<Enum, int>(); // Материалы на заводе
        public Dictionary<Enum, int> ProductsBank = new Dictionary<Enum, int>(); // Продукция на заводе
        public List<Workshop> Workshops = new(); // Список цехов на заводе

        public IndustrialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
            Type = type;
        }


        /// <summary>
        /// Добавить новый цех.
        /// </summary>
        public void AddWorkshop(Enum input, Enum output, int coeff = 1)
        {
            var ws = new Workshop(this, input, output, coeff);
            Workshops.Add(ws);
        }

        /// <summary>
        /// Запустить все цехи один раз.
        /// </summary>
        public void RunOnce()
        {
            foreach (var ws in Workshops)
                ws.Process();
        }
    }
}
